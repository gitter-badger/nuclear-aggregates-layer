using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        public static bool IsErmAssembly(AssemblyName checkingAssemblyName)
        {
            const string ErmAssemblyNameTemplate = "2Gis.";

            return checkingAssemblyName.Name.StartsWith(ErmAssemblyNameTemplate);
        }

        public static bool IsErmAssembly(Assembly checkingAssembly)
        {
            return IsErmAssembly(checkingAssembly.GetName());
        }

        public static void PerfomTypesMassProcessings(IEnumerable<Assembly> assemblies, IMassProcessor[] massProcessors, bool firstRun, Type adaptationMarkerType)
        {
            var exportedTypesMap = GetExportedTypesMap(assemblies, adaptationMarkerType);

            foreach (var massProcessor in massProcessors)
            {
                var assignnableTypes = massProcessor.GetAssignableTypes();

                var exportedTypes = assignnableTypes.SelectMany(x => exportedTypesMap[x]).ToArray();
                if (!exportedTypes.Any())
                {
                    throw new ApplicationException(string.Format("Cannot find any types for massprocessor '{0}'", massProcessor.GetType().Name));
                }

                massProcessor.ProcessTypes(exportedTypes, firstRun);
                massProcessor.AfterProcessTypes(firstRun);
            }
        }

        public static void PerfomTypesMassProcessings(IMassProcessor[] massProcessors, bool firstRun, Type adaptationMarkerType)
        {
            PerfomTypesMassProcessings(AppDomain.CurrentDomain.GetAssemblies(), massProcessors, firstRun, adaptationMarkerType);
        }

        private static Dictionary<Type, Type[]> GetExportedTypesMap(IEnumerable<Assembly> assemblies, Type adaptationMarkerType)
        {
            try
            {
                var exportedTypesMap = assemblies
                    .Select(x => new
                        {
                            Assembly = x,
                            ContainsTypes = ((ContainedTypesAttribute[])x.GetCustomAttributes(typeof(ContainedTypesAttribute), false)).SelectMany(y => y.Types),
                        })
                    .Where(x => x.ContainsTypes.Any())
                    .SelectMany(x => x.ContainsTypes.Select(y => new
                        {
                            AssignableType = y,
                            Types = x.Assembly.GetExportedTypes()
                                     .Where(z => y != z && y.IsAssignableFrom(z))
                                     // фильтруем неадаптированные типы
                                     .Where(z => !(typeof(IAdapted).IsAssignableFrom(z) && !adaptationMarkerType.IsAssignableFrom(z))),
                        }))
                    .GroupBy(x => x.AssignableType, x => x.Types)
                    .ToDictionary(x => x.Key, x => x.SelectMany(y => y).ToArray());

                return exportedTypesMap;
            }
            catch (ReflectionTypeLoadException ex)
            {
                var message = ex.LoaderExceptions.Aggregate("Ошибка при загрузке типов, loaderExceptions: ", (x, y) => x + y.ToString());
                throw new ApplicationException(message, ex);
            }
        }
    }
}
