using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata
{
    public interface IExtendedInfoFilterMetadata
    {
        void RegisterExtendedInfoFilter<TDocument, TInfoType>(string filterName, Func<TInfoType, Expression<Func<TDocument, bool>>> func);
        IReadOnlyCollection<Expression<Func<TDocument, bool>>> GetExtendedInfoFilters<TDocument>(IReadOnlyDictionary<string, string> extendedInfoMap);
    }
}