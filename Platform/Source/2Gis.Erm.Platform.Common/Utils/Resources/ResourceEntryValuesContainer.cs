using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public sealed class ResourceEntryValuesContainer
    {
        private readonly IDictionary<CultureInfo, object> _valuesMap = new Dictionary<CultureInfo, object>();

        public ResourceEntryValuesContainer Add(CultureInfo entryCulture, object entryValue)
        {
            _valuesMap.Add(entryCulture, entryValue);
            return this;
        }

        public IReadOnlyDictionary<CultureInfo, object> ValuesMap
        {
            get { return new ReadOnlyDictionary<CultureInfo, object>(_valuesMap); }
        }
    }
}