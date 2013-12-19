﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueprintViewModel.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace BlueprintEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using BlueprintEditor.Annotations;

    using MonitoredUndo;

    using Slash.GameBase.Blueprints;

    public class BlueprintViewModel : INotifyPropertyChanged, ISupportsUndo, IDataErrorInfo
    {
        #region Fields

        private IEnumerable<Type> assemblyComponents;

        /// <summary>
        ///   Id of the blueprint.
        /// </summary>
        private string blueprintId;

        /// <summary>
        ///   New blueprint id.
        /// </summary>
        private string newBlueprintId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor.
        /// </summary>
        /// <param name="blueprintId">Blueprint id.</param>
        /// <param name="blueprint">Blueprint.</param>
        public BlueprintViewModel(string blueprintId, Blueprint blueprint)
        {
            this.BlueprintId = this.newBlueprintId = blueprintId;
            this.Blueprint = blueprint;

            // Set added components.
            this.AddedComponents = new ObservableCollection<Type>(this.Blueprint.ComponentTypes);
            this.AddedComponents.CollectionChanged += this.OnAddedComponentsChanged;

            // Set available components.
            this.AvailableComponents = new ObservableCollection<Type>();
            this.AvailableComponents.CollectionChanged += this.OnAvailableComponentsChanged;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Components which are already added to the blueprint.
        /// </summary>
        public ObservableCollection<Type> AddedComponents { get; private set; }

        /// <summary>
        ///   All available entity component types which can be added to a blueprint.
        /// </summary>
        public IEnumerable<Type> AssemblyComponents
        {
            get
            {
                return this.assemblyComponents;
            }
            set
            {
                this.assemblyComponents = value;

                this.UpdateAvailableComponents();
            }
        }

        /// <summary>
        ///   Components which can be added to the blueprint.
        /// </summary>
        public ObservableCollection<Type> AvailableComponents { get; private set; }

        /// <summary>
        ///   Blueprint this item represents.
        /// </summary>
        public Blueprint Blueprint { get; private set; }

        /// <summary>
        ///   Id of the blueprint.
        /// </summary>
        public string BlueprintId
        {
            get
            {
                return this.blueprintId;
            }
            set
            {
                if (value == this.blueprintId)
                {
                    return;
                }

                this.blueprintId = value;

                this.OnPropertyChanged();
            }
        }

        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///   Id of the blueprint.
        /// </summary>
        public string NewBlueprintId
        {
            get
            {
                return this.newBlueprintId;
            }
            set
            {
                if (value == this.newBlueprintId)
                {
                    return;
                }

                this.newBlueprintId = value;

                this.OnPropertyChanged();
            }
        }

        public BlueprintManagerViewModel Root { get; set; }

        #endregion

        #region Public Indexers

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == "NewBlueprintId")
                {
                    if (this.Root != null)
                    {
                        BlueprintViewModel blueprintViewModel =
                            this.Root.Blueprints.FirstOrDefault(
                                viewModel => viewModel.BlueprintId == this.NewBlueprintId);
                        if (blueprintViewModel != null && blueprintViewModel != this)
                        {
                            result = "Blueprint id already exists.";
                        }
                        else if (this.newBlueprintId != this.blueprintId)
                        {
                            // Move in blueprint manager.
                            this.Root.ChangeBlueprintId(this, this.newBlueprintId);
                        }
                    }
                }
                return result;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void AddComponent(Type componentType)
        {
            if (this.Blueprint.ComponentTypes.Contains(componentType))
            {
                throw new ArgumentException(
                    string.Format("Component type '{0}' already added to blueprint.", componentType.Name),
                    "componentType");
            }

            // Remove from available, add to blueprint component types.
            this.AvailableComponents.Remove(componentType);
            this.AddedComponents.Add(componentType);
        }

        public object GetUndoRoot()
        {
            return this.Root;
        }

        public bool RemoveComponent(Type componentType)
        {
            // Remove from available, add to blueprint component types.
            if (this.AssemblyComponents.Contains(componentType))
            {
                this.AvailableComponents.Add(componentType);
            }

            return this.AddedComponents.Remove(componentType);
        }

        #endregion

        #region Methods

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnAddedComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Add component types to blueprint.
                foreach (Type item in e.NewItems)
                {
                    this.Blueprint.ComponentTypes.Add(item);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // Remove component types from blueprint.
                foreach (Type item in e.OldItems)
                {
                    this.Blueprint.ComponentTypes.Remove(item);
                }
            }
        }

        private void OnAvailableComponentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("AvailableComponents");
        }

        private void UpdateAvailableComponents()
        {
            if (this.assemblyComponents == null)
            {
                this.AvailableComponents.Clear();
                return;
            }

            // Update available components.
            IEnumerable<Type> newAvailableComponents =
                this.assemblyComponents.Except(this.Blueprint.ComponentTypes).ToList();

            foreach (var availableComponent in this.AvailableComponents)
            {
                if (!newAvailableComponents.Contains(availableComponent))
                {
                    this.AvailableComponents.Remove(availableComponent);
                }
            }

            foreach (var newAvailableComponent in newAvailableComponents)
            {
                if (!this.AvailableComponents.Contains(newAvailableComponent))
                {
                    this.AvailableComponents.Add(newAvailableComponent);
                }
            }
        }

        #endregion
    }
}