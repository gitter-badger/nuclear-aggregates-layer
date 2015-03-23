using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public static class ResourceUtils
    {
        public static bool TryResolveResourceManager(this Type resourceEntryHostType, out ResourceManager resourceManager)
        {
            if (TryResolveResourceManagerUsingCtor(resourceEntryHostType, out resourceManager))
            {
                return true;
            }

            if (TryResolveResourceManagerUsingReflection(resourceEntryHostType, out resourceManager))
            {
                return true;
            }

            return false;
        }

        public static ResourceManager AsResourceManager(this Type resourceEntryHostType)
        {
            ResourceManager resourceManager;
            if (!TryResolveResourceManager(resourceEntryHostType, out resourceManager))
            {
                throw new InvalidOperationException("Can't resolve resource manager from specified entry host type " + resourceEntryHostType);
            }

            return resourceManager;
        }

        public static IReadOnlyDictionary<string, ResourceEntriesRegistry> EvaluateAvailableResources(
            this IEnumerable<Type> resourseManagerHostTypes,
            bool useFallbackBehavior,
            params CultureInfo[] targetCultures)
        {
            var keys2EntriesMap = new Dictionary<string, ResourceEntriesRegistry>();

            foreach (var resourceManagerHostType in resourseManagerHostTypes)
            {
                var resourceManager = resourceManagerHostType.AsResourceManager();

                foreach (var targetCulture in targetCultures)
                {
                    var cultureSpecificResourceSet = resourceManager.GetResourceSet(targetCulture, true, useFallbackBehavior);
                    if (cultureSpecificResourceSet == null)
                    {
                        continue;
                    }

                    foreach (var entry in cultureSpecificResourceSet.Cast<DictionaryEntry>())
                    {
                        var entryKey = (string)entry.Key;
                        ResourceEntriesRegistry entriesRegistry;
                        if (!keys2EntriesMap.TryGetValue(entryKey, out entriesRegistry))
                        {
                            entriesRegistry = new ResourceEntriesRegistry();
                            keys2EntriesMap.Add(entryKey, entriesRegistry);
                        }

                        entriesRegistry.Add(new ResourceEntryKey(resourceManagerHostType, entryKey), targetCulture, entry.Value);
                    }
                }
            }

            return new ReadOnlyDictionary<string, ResourceEntriesRegistry>(keys2EntriesMap);
        }

        public static bool TryGetDuplicatedResourceEntry(this IEnumerable<KeyValuePair<string, ResourceEntriesRegistry>> entriesRegistry, out string report)
        {
            var sb = new StringBuilder();
            foreach (var entry in entriesRegistry.Where(e => e.Value.Entries.Count > 1))
            {
                sb.AppendFormat("ResourceEntry:{0} is duplicated in several resources: {1}\n",
                                entry.Key,
                                string.Join(";", entry.Value.Entries.Select(x => x.Key.ResourceHostType)));
            }

            report = sb.Length > 0 ? sb.ToString() : null;
            return report != null;
        }

        private static bool TryResolveResourceManagerUsingCtor(Type resourceEntryHostType, out ResourceManager resourceManager)
        {
            resourceManager = null;

            try
            {
                resourceManager = new ResourceManager(resourceEntryHostType);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryResolveResourceManagerUsingReflection(Type resourceEntryHostType, out ResourceManager resourceManager)
        {
            resourceManager = null;

            const string ResoureManagerPropertyName = "ResourceManager";
            var resourceManagerProperty = resourceEntryHostType.GetProperty(ResoureManagerPropertyName, BindingFlags.Public | BindingFlags.Static);
            if (resourceManagerProperty == null)
            {
                return false;
            }

            resourceManager = (ResourceManager)resourceManagerProperty.GetValue(null);
            return resourceManager != null;
        }
    }
}
