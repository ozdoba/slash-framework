﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportDataCSVWindow.xaml.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BlueprintEditor.Windows
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    using BlueprintEditor.Controls;
    using BlueprintEditor.ViewModels;

    using Slash.Tools.BlueprintEditor.Logic.Data;

    public partial class ImportDataCSVWindow
    {
        #region Fields

        private readonly IEnumerable<string> csvColumnHeaders;

        private readonly List<ValueMappingViewModel> valueMappings = new List<ValueMappingViewModel>();

        #endregion

        #region Constructors and Destructors

        public ImportDataCSVWindow(EditorContext context, IEnumerable<string> csvColumnHeaders)
        {
            this.InitializeComponent();

            this.CbBlueprintIdMapping.DataContext = this;

            this.CbBlueprints.DataContext = context;
            this.CbBlueprints.PropertyChanged += this.OnSelectedParentBlueprintChanged;

            this.csvColumnHeaders = csvColumnHeaders;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   CSV column that contains the blueprint ids for the blueprints to create.
        /// </summary>
        public string BlueprintIdColumn
        {
            get
            {
                return (string)this.CbBlueprintIdMapping.SelectedItem;
            }
        }

        /// <summary>
        ///   Parent blueprint of the blueprints to create.
        /// </summary>
        public BlueprintViewModel BlueprintParent
        {
            get
            {
                return this.CbBlueprints.SelectedItem;
            }
        }

        /// <summary>
        ///   Headers of the columns of the imported CSV file.
        /// </summary>
        public ObservableCollection<string> CSVColumnHeaders
        {
            get
            {
                return new ObservableCollection<string>(this.csvColumnHeaders);
            }
        }

        /// <summary>
        ///   Maps attribute table keys to CSV columns.
        /// </summary>
        public ReadOnlyCollection<ValueMappingViewModel> ValueMappings
        {
            get
            {
                return new ReadOnlyCollection<ValueMappingViewModel>(this.valueMappings);
            }
        }

        #endregion

        #region Methods

        private void AddAttributeMappingsRecursively(BlueprintViewModel viewModel)
        {
            // Add mapping controls for parent blueprints.
            if (viewModel.Parent != null)
            {
                this.AddAttributeMappingsRecursively(viewModel.Parent);
            }

            // Add mapping controls for specified blueprint.
            foreach (var componentType in viewModel.AddedComponents)
            {
                // Get attributes.
                var componentInfo = InspectorComponentTable.Instance.GetInspectorType(componentType);
                if (componentInfo == null)
                {
                    continue;
                }

                foreach (var inspectorProperty in componentInfo.Properties)
                {
                    // Create attribute mapping control.
                    ValueMappingViewModel valueMapping = new ValueMappingViewModel
                        {
                            MappingSource = inspectorProperty.Name,
                            AvailableMappingTargets = new ObservableCollection<string>(this.csvColumnHeaders)
                        };

                    ValueMappingControl valueMappingControl = new ValueMappingControl(valueMapping);

                    // Add to panel.
                    this.valueMappings.Add(valueMapping);
                    this.SpAttributeMapping.Children.Add(valueMappingControl);
                }
            }
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ImportData_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void OnSelectedParentBlueprintChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateAttributeMapping();
        }

        private void UpdateAttributeMapping()
        {
            // Clear attribute mapping controls.
            this.valueMappings.Clear();
            this.SpAttributeMapping.Children.Clear();

            // Add new attribute mapping controls.
            this.AddAttributeMappingsRecursively(this.CbBlueprints.SelectedItem);
        }

        #endregion
    }
}