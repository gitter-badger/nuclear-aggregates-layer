using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;

using NuClear.Settings.API;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder
{
    public class ModulesFinder : IModulesFinder
    {
        private const string DefaultAssemblyPattern = "{0}.dll";

        private readonly IDictionary<string, IEnumerable<string>> _files;

        public ModulesFinder(string directoryPath, string searchPattern, SearchOption searchOption)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new InvalidOperationException("Specified directory " + directoryPath + " for modules doesn't exists");
            }

            var defaultSearchPattern = string.Format(DefaultAssemblyPattern, "*");
            _files = Directory.GetFiles(directoryPath, !string.IsNullOrEmpty(searchPattern) ? searchPattern : defaultSearchPattern, searchOption)
                              .GroupBy(Path.GetFileName)
                              .ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        public IEnumerable<ModulesDescriptor> GetModulesDescriptors()
        {
            if (!_files.Any())
            {
                return new ModulesDescriptor[0];
            }

            var modulesDescriptors = new List<ModulesDescriptor>();
            var findModulesAppDomain = AppDomain.CreateDomain("FindModulesAppDomain",
                                                              new Evidence(AppDomain.CurrentDomain.Evidence),
                                                              AppDomain.CurrentDomain.SetupInformation);

            AssemblyLoadProxy assemblyLoadProxy = null;
            try
            {
                assemblyLoadProxy = (AssemblyLoadProxy)findModulesAppDomain.CreateInstanceAndUnwrap(typeof(AssemblyLoadProxy).Assembly.FullName,
                                                                                                    typeof(AssemblyLoadProxy).FullName);
                
                var assembliesWithoutSattelites = _files.Where(x => x.Value.Count() == 1).SelectMany(x => x.Value).ToArray();
                foreach (var fileFullPath in assembliesWithoutSattelites)
                {
                    AssemblyName assemblyName;
                    if (!TryGetAssemblyName(fileFullPath, out assemblyName) || assemblyName == null)
                    {
                        continue;
                    }

                    var descriptor = assemblyLoadProxy.GetModuleDescriptor(fileFullPath);
                    if (descriptor != null)
                    {
                        modulesDescriptors.Add(descriptor);
                    }
                }
            }
            finally 
            {
                if (assemblyLoadProxy != null)
                {
                    assemblyLoadProxy.Dispose();
                }

                AppDomain.Unload(findModulesAppDomain);
            }

            return modulesDescriptors;
        }

        public string GetAssemblyPath(AssemblyName assemblyName)
        {
            var assemblyFileName = string.Format(DefaultAssemblyPattern, assemblyName.Name);
            return _files.ContainsKey(assemblyFileName) ? _files[assemblyFileName].First() : null;
        }

        private static bool TryGetAssemblyName(string assemblyFilePath, out AssemblyName assemblyName)
        {
            assemblyName = null;

            try
            {
                assemblyName = AssemblyName.GetAssemblyName(assemblyFilePath);
            }
            catch (BadImageFormatException)
            {
                // файл не .NET сборка 
                return false;
            }

            return true;
        }

        private class AssemblyLoadProxy : MarshalByRefObject, IDisposable
        {
            public AssemblyLoadProxy()
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            }

            public ModulesDescriptor GetModuleDescriptor(string assemblyPath)
            {
                return GetDescriptorForAssembly(assemblyPath);
            }

            public void Dispose()
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            }

            private static ModulesDescriptor GetDescriptorForAssembly(string assemblyFullPath)
            {
                var assembly = Assembly.LoadFrom(assemblyFullPath);
                // TODO {all, 30.07.2013}: лучше избавиться от упоминаний 2gis, ERM и т.д. в common wpf сборке, т.к. здесь инфраструктурный код загрузки, общий для любого .net (например, WPF) приложения
                // всю erm специфику лучше передавать в виде настроек/параметров 
                if (!assembly.FullName.StartsWith("2gis", StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                var exportedTypes = assembly.GetExportedTypes();

                var modulesContainers = exportedTypes
                                            .Where(t => t != ModuleIndicators.Container && ModuleIndicators.Container.IsAssignableFrom(t))
                                            .ToArray();
                var modules = exportedTypes
                                .Where(t => !ModuleIndicators.Group.All.Contains(t) && ModuleIndicators.Module.IsAssignableFrom(t))
                                .ToArray();

                if (modulesContainers.Length == 0 || modules.Length == 0)
                {
                    return null;
                }

                if (modulesContainers.Length > 1)
                {
                    throw new InvalidOperationException(string.Format("Invalid modules container count: {0} detected in file: {1}",
                                                                      modulesContainers.Length,
                                                                      assemblyFullPath));
                }

                var settings = exportedTypes
                    .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType && t.IsSettings())
                    .Select(impl => new SettingsContainerDescriptor { Implementation = impl.AssemblyQualifiedName })
                    .ToArray();

                var modulesContainerConfigFullPath = string.Format("{0}.dll.config",
                                                                   Path.Combine(Path.GetDirectoryName(assemblyFullPath),
                                                                                Path.GetFileNameWithoutExtension(assemblyFullPath)));
                return new ModulesDescriptor
                    {
                        ModulesContainerFullPath = assemblyFullPath,
                        ModulesContainerConfigFullPath = File.Exists(modulesContainerConfigFullPath) ? modulesContainerConfigFullPath : null,
                        ModulesContainerAssembly = assembly.FullName,
                        ContainerType = modulesContainers.Single().AssemblyQualifiedName,
                        ModuleTypes = modules.Select(x => x.AssemblyQualifiedName).ToArray(),
                        Settings = settings
                    };

            }

            private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
            {
                var targetName = new AssemblyName(args.Name);
                if (targetName.GetPublicKeyToken() == null)
                {
                    return null;
                }

                var searchPattern = string.Format("{0}.dll", targetName.Name);
                var assemblyFiles = Directory.GetFiles(Path.GetDirectoryName(args.RequestingAssembly.Location), searchPattern, SearchOption.AllDirectories);
                if (assemblyFiles.Length == 0 || assemblyFiles.Length > 1)
                {
                    return Assembly.Load(args.Name);
                }

                return Assembly.LoadFrom(assemblyFiles.First());
            }
        }
    }
}