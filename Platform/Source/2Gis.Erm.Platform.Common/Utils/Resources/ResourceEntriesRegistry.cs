using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public sealed class ResourceEntriesRegistry
    {
        private readonly IDictionary<ResourceEntryKey, ResourceEntryValuesContainer> _entryRegistry = 
            new Dictionary<ResourceEntryKey, ResourceEntryValuesContainer>();

        public ResourceEntriesRegistry Add(ResourceEntryKey entryKey, CultureInfo entryCulture, object entryValue)
        {
            ResourceEntryValuesContainer container;
            if (!_entryRegistry.TryGetValue(entryKey, out container))
            {
                container = new ResourceEntryValuesContainer();
                _entryRegistry.Add(entryKey, container);
            }

            container.Add(entryCulture, entryValue);

            return this;
        }

        public IDictionary<ResourceEntryKey, ResourceEntryValuesContainer> Entries
        {
            get { return new ReadOnlyDictionary<ResourceEntryKey, ResourceEntryValuesContainer>(_entryRegistry); }
        }
    }
}