using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;

using NuClear.ResourceUtilities;
using NuClear.ResourceUtilities.Legacy;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ResourceHandling
{
    /// <summary>
    /// Вспомогательный класс, используется в t4-преобразовании JsResourcesGeneration
    /// </summary>
    public sealed class ResourcesResolver
    {
        private readonly IReadOnlyDictionary<string, ResourceEntriesRegistry> _availableResources;

        public ResourcesResolver(IEnumerable<Type> resourceEntryHostTypes, IEnumerable<CultureInfo> targetCultures)
        {
            _availableResources = resourceEntryHostTypes.EvaluateAvailableResources(false, targetCultures.ToArray());
            // TODO {all, 24.12.2013}: пока проверка на корректность ресурсников продублирована и здесь, однако, когда появится massprocessor для ресурсников, необходимость в такой проверке скорее всего отпадет
            CheckResourcesConsistency(_availableResources);
        }

        public IReadOnlyDictionary<string, string> ExtractResources(CultureInfo cultureInfo, out string report)
        {
            var extractedResources = new Dictionary<string, string>();
            var processingDetails =
                new StringBuilder()
                    .AppendLine("Processing resources report:")
                    .AppendFormat("\tTarget culture: {0}", cultureInfo).Append(Environment.NewLine)
                    .AppendFormat("\tResource entries count: {0}", _availableResources.Count).Append(Environment.NewLine);

            foreach (var availableResource in _availableResources)
            {
                var currentResourceEntry = availableResource.Value.Entries.Single();
                var resourceEntryDetails = currentResourceEntry.Value;

                object resourceEntryValue;
                if (resourceEntryDetails.ValuesMap.TryGetValue(cultureInfo, out resourceEntryValue))
                {
                    extractedResources.Add(availableResource.Key, (string)resourceEntryValue);
                    continue;
                }
                
                var unresolvedValueStub = string.Format("__UNRESOLVED__{0}__", currentResourceEntry.Key);
                ResourceManager resourceManager;
                if (!currentResourceEntry.Key.ResourceHostType.TryResolveResourceManager(out resourceManager))
                {
                    processingDetails
                        .AppendFormat("Can't resolve resource entry {0} value for requested culture, resource manager for fallback value is unresolved, stub value used: {1}",
                                      currentResourceEntry.Key,
                                      unresolvedValueStub)
                        .Append(Environment.NewLine);

                    extractedResources.Add(availableResource.Key, unresolvedValueStub);

                    continue;
                }

                var fallbackResourceEntryValue = resourceManager.GetString(availableResource.Key, cultureInfo);
                if (fallbackResourceEntryValue == null)
                {
                    processingDetails
                        .AppendFormat("Can't resolve resource entry {0} value for requested culture, fallback resource entry value is null, stub value used: {1}",
                                      currentResourceEntry.Key,
                                      unresolvedValueStub)
                        .Append(Environment.NewLine);

                    extractedResources.Add(availableResource.Key, unresolvedValueStub);

                    continue;
                }

                processingDetails
                        .AppendFormat("Can't resolve resource entry {0} value for requested culture, fallback resource entry value used: {1}",
                                      currentResourceEntry.Key,
                                      fallbackResourceEntryValue)
                        .Append(Environment.NewLine);

                extractedResources.Add(availableResource.Key, fallbackResourceEntryValue);
            }

            report = processingDetails.ToString();
            return extractedResources;
        }
        
        private void CheckResourcesConsistency(IEnumerable<KeyValuePair<string, ResourceEntriesRegistry>> availableResources)
        {
            string report;
            if (availableResources.TryGetDuplicatedResourceEntry(out report))
            {
                throw new InvalidOperationException("Resources usage conventions violated. Duplicated resource entries detected. " + report);
            }
        }
    }
}
