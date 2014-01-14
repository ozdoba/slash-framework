﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyUtils.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.Reflection.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Windows.ApplicationModel;
    using Windows.Foundation;
    using Windows.Storage;

    public class AssemblyUtils
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets all assemblies that are loaded in the current application domain.
        /// </summary>
        /// <returns>All loaded assemblies.</returns>
        public static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            StorageFolder folder = Package.Current.InstalledLocation;

            List<Assembly> assemblies = new List<Assembly>();

            IAsyncOperation<IReadOnlyList<StorageFile>> folderFilesAsync = folder.GetFilesAsync();
            folderFilesAsync.AsTask().Wait();

            foreach (StorageFile file in folderFilesAsync.GetResults())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    try
                    {
                        var filename = file.Name.Substring(0, file.Name.Length - file.FileType.Length);
                        AssemblyName name = new AssemblyName { Name = filename };
                        Assembly asm = Assembly.Load(name);
                        assemblies.Add(asm);
                    }
                    catch (BadImageFormatException)
                    {
                        /*
                         * TODO(np): Thrown reflecting on C++ executable files for which the C++ compiler
                         * stripped the relocation addresses (such as Unity dlls):
                         * http://msdn.microsoft.com/en-us/library/x4cw969y(v=vs.110).aspx
                         */
                    }
                }
            }

            return assemblies;
        }

        #endregion
    }
}