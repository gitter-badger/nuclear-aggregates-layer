using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace NuClear.Metamodeling.UI.Utils.Resources
{
    public sealed class ResourceEntryValuesContainer
    {
        private readonly IDictionary<CultureInfo, object> _valuesMap = new Dictionary<CultureInfo, object>();


        public IReadOnlyDictionary<CultureInfo, object> ValuesMap
        {
            get { return new ReadOnlyDictionary<CultureInfo, object>(_valuesMap); }
        }

        public ResourceEntryValuesContainer Add(CultureInfo entryCulture, object entryValue)
        {
            _valuesMap.Add(entryCulture, entryValue);
            return this;
        }
    }
}