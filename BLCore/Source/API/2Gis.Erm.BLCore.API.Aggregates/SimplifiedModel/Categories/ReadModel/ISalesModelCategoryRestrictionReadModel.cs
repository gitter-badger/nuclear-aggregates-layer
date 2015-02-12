using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public interface ISalesModelCategoryRestrictionReadModel : ISimplifiedModelConsumerReadModel
    {
        IEnumerable<SalesModelCategoryRestriction> GetRestrictionsByProject(long projectId);
        IReadOnlyCollection<long> GetDependedByRestrictionsInProjectOrderIds(long projectId);
    }
}