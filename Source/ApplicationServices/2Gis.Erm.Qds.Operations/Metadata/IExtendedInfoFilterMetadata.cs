using System;
using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public interface IQdsExtendedInfoFilterMetadata
    {
        void RegisterExtendedInfoFilter<TDocument, TInfoType>(string filterName, Func<TInfoType, Func<FilterDescriptor<TDocument>, FilterContainer>> func) where TDocument : class;
        IReadOnlyCollection<Func<FilterDescriptor<TDocument>, FilterContainer>> GetExtendedInfoFilters<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap) where TDocument : class;
        bool ContainsExtendedInfo<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap);
    }
}