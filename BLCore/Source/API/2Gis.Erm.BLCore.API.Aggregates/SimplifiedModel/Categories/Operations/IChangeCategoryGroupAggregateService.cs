using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Categories;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.Operations
{
    public interface IChangeCategoryGroupAggregateService : ISimplifiedModelConsumer
    {
        void Change(IEnumerable<CategoryOrganizationUnitDto> dtos, IDictionary<long, long?> categoryToGroupMapping);
    }
}