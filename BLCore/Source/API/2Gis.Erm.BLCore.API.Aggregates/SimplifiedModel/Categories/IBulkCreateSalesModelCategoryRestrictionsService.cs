using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories
{
    public interface IBulkCreateSalesModelCategoryRestrictionsService : ISimplifiedModelConsumer
    {
        void Create(IEnumerable<SalesModelCategoryRestriction> recordsToCreate);
    }
}