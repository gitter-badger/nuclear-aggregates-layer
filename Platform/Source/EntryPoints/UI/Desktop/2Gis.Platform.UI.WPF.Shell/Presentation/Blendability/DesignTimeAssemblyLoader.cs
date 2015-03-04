using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Nuclear.Tracing.API;

namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Blendability
{
    public static class DesignTimeAssemblyLoader
    {
        private readonly static Dictionary<string, List<Assembly>> AssemblyCache = new Dictionary<string, List<Assembly>>();

        static DesignTimeAssemblyLoader()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                List<Assembly> assembliesList;
                if (!AssemblyCache.TryGetValue(assembly.GetName().Name, out assembliesList))
                {
                    assembliesList = new List<Assembly>();
                    AssemblyCache.Add(assembly.GetName().Name, assembliesList);
                }
                assembliesList.Add(assembly);
            }
        }

        private static ITracer Logger { get; set; }

        public static void Attach(ITracer logger)
        {
            Logger = logger;
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        public static void Deattach()
        {
            AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
            Logger = null;
        }

        public static Assembly AssemblyLoaderToNoLoadContext(AssemblyName targetAssemblyName, string targetAssemblyFileFullPath)
        {
            List<Assembly> assembliesList;
            if (AssemblyCache.TryGetValue(targetAssemblyName.Name, out assembliesList))
            {
                foreach (var assembly in assembliesList)
                {
                    if (string.CompareOrdinal(assembly.FullName, targetAssemblyName.FullName) == 0)
                    {
                        return assembly;
                    }
                }
            }

            Logger.Debug("Loading to no load context. " + targetAssemblyFileFullPath);
            return Assembly.LoadFile(targetAssemblyFileFullPath);
        }
        
        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            List<Assembly> assembliesList;
            var loadedAssemblyName = args.LoadedAssembly.GetName();
            if (!AssemblyCache.TryGetValue(loadedAssemblyName.Name, out assembliesList))
            {
                assembliesList = new List<Assembly> { args.LoadedAssembly };
                AssemblyCache.Add(loadedAssemblyName.Name, assembliesList);
                return;
            }
            
            if (assembliesList.Any(assembly => string.CompareOrdinal(assembly.FullName, loadedAssemblyName.FullName) == 0))
            {
                return;
            }

            assembliesList.Add(args.LoadedAssembly);
        }
        
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            List<Assembly> assembliesList;
            var targetAssemblyName = new AssemblyName(args.Name);
            const string CatelAssembliesIndicator = "Catel";
            if (targetAssemblyName.Name.StartsWith(CatelAssembliesIndicator))
            {
                var publicKeyToken = targetAssemblyName.GetPublicKeyToken();
                if (publicKeyToken == null || publicKeyToken.Length == 0)
                {
                    Logger.Debug("Custom override catel not signed dependencies. " + targetAssemblyName);
                    if (!AssemblyCache.TryGetValue(targetAssemblyName.Name, out assembliesList))
                    {
                        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), targetAssemblyName));
                    }

                    return assembliesList.FirstOrDefault();
                }
            }
            
            if (AssemblyCache.TryGetValue(targetAssemblyName.Name, out assembliesList))
            {
                foreach (var assembly in assembliesList)
                {
                    if (string.Compare(assembly.FullName, targetAssemblyName.FullName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return assembly;
                    }
                }
            }

            if (args.RequestingAssembly == null)
            {
                Logger.Debug("Can't resolve assembly. Requesting assembly is not specified. " + targetAssemblyName);
                return null;
            }

            // считаем, что сборка, которую нужно загрузить и сборка из-за reference в которой понадобилось загружать сборку, находятся в одной и той же директории
            var referencingAssemblyPath = Path.GetDirectoryName(args.RequestingAssembly.GetModules()[0].FullyQualifiedName);
            var targetAssemblyFileFullPath = Path.Combine(referencingAssemblyPath, targetAssemblyName.Name + ".dll");
            return AssemblyLoaderToNoLoadContext(targetAssemblyName, targetAssemblyFileFullPath);
            
        }
    }
}
