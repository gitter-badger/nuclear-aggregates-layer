using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Unity;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static class PlatformBootstrapper
    {
        public static IUnityContainer ConfigureSettingsAspects(this IUnityContainer unityContainer, ISettingsContainer settingsContainer)
        {
            var settingsContractsMap = new Dictionary<Type, List<ISettings>>();
            ExtractSettingsContracts(settingsContainer, settingsContractsMap);

            var duplicatedImplementaions = settingsContractsMap.Where(pair => pair.Value.Count > 1).ToArray();
            if (duplicatedImplementaions.Any())
            {
                string report = duplicatedImplementaions.Aggregate(new StringBuilder(), (builder, pair) => builder.Append(pair.Key.FullName + "; ")).ToString();
                throw new InvalidOperationException("Found several settings aspects implementations, with duplicated settings contracts. Duplicated settings contracts list: " + report);
            }

            foreach (var settingsBucket in settingsContractsMap)
            {
                unityContainer.RegisterInstance(settingsBucket.Key, settingsBucket.Value.Single());
            }

            return unityContainer;
        }

        private static void ExtractSettingsContracts(ISettingsContainer settingsContainer, Dictionary<Type, List<ISettings>> settingsContractsMap)
        {
            settingsContainer
                .GetSettingsContracts()
                .Aggregate(settingsContractsMap, (map, type) => Add2SettingsContractsMap(map, type, settingsContainer));

            foreach (var aspect in settingsContainer.SettingsAspects)
            {
                var childContainer = aspect as ISettingsContainer;
                if (childContainer != null)
                {
                    ExtractSettingsContracts(childContainer, settingsContractsMap);
                    continue;
                }

                aspect
                   .GetSettingsContracts()
                   .Aggregate(settingsContractsMap, (map, type) => Add2SettingsContractsMap(map, type, aspect));
            }
        }

        private static IEnumerable<Type> GetSettingsContracts(this ISettings settingHost)
        {
            return settingHost
                        .GetType()
                        .GetInterfaces()
                        .Where(t => t.IsSettings() && !SettingsIndicators.Group.All.Contains(t))
                        .Distinct();
        }

        private static Dictionary<Type, List<ISettings>> Add2SettingsContractsMap(
            Dictionary<Type, List<ISettings>> settingsContractsMap,
            Type settingsContract,
            ISettings settingHost)
        {
            List<ISettings> implementations;
            if (!settingsContractsMap.TryGetValue(settingsContract, out implementations))
            {
                implementations = new List<ISettings>();
                settingsContractsMap.Add(settingsContract, implementations);
            }

            implementations.Add(settingHost);

            return settingsContractsMap;
        }
    }
}
