using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.DI.Zones;
using DoubleGis.Erm.Platform.Model.Zones;

using NuClear.Assembling.TypeProcessing;

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

        public static void PerformTypesMassProcessing(CompositionRoot root,
                                                      IMassProcessor[] massProcessors,
                                                      bool firstRun,
                                                      IBusinessModelSettings businessModelSettings)
        {
            var exportedTypesMap = GetExportedTypesMap(root, businessModelSettings);

            foreach (var massProcessor in massProcessors)
            {
                var assignableTypes = massProcessor.GetAssignableTypes();

                var exportedTypes = assignableTypes.Where(exportedTypesMap.ContainsKey).SelectMany(x => exportedTypesMap[x]).ToArray();
                if (!exportedTypes.Any())
                {
                    throw new ApplicationException(string.Format("Cannot find any types for massprocessor '{0}'", massProcessor.GetType().Name));
                }

                massProcessor.ProcessTypes(exportedTypes, firstRun);
                massProcessor.AfterProcessTypes(firstRun);
            }
        }

        private static Dictionary<Type, Type[]> GetExportedTypesMap(CompositionRoot root, IBusinessModelSettings businessModelSettings)
        {
            try
            {
                var zoneTypesGrouping = from zoneDescriptor in root.ZoneDescriptors
                                        from zoneAssembly in zoneDescriptor.Assemblies
                                        from type in zoneAssembly.ExportedTypes
                                        where type.IsZonePartMarker(zoneDescriptor.ZoneType)
                                        let zonePartInfo = type.GetZonePartInfo()
                                        where zonePartInfo.BusinessModel == null || zonePartInfo.BusinessModel == businessModelSettings.BusinessModelIndicator
                                        from pair in zonePartInfo.ExtractTypesMap(businessModelSettings.BusinessModelIndicator)
                                        group pair by pair.Key
                                        into g
                                        select new { AssignableType = g.Key, Types = g.SelectMany(x => x.Value).ToArray() };

                return zoneTypesGrouping.ToDictionary(x => x.AssignableType, x => x.Types);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var message = ex.LoaderExceptions.Aggregate("Ошибка при загрузке типов, loaderExceptions: ", (x, y) => x + y.ToString());
                throw new ApplicationException(message, ex);
            }
        }
    }
}