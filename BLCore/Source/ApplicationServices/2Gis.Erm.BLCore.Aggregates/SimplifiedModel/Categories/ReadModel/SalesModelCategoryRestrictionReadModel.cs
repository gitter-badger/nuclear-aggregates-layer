using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.Categories.ReadModel
{
    public class SalesModelCategoryRestrictionReadModel : ISalesModelCategoryRestrictionReadModel
    {
        private readonly IFinder _finder;

        public SalesModelCategoryRestrictionReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public IEnumerable<SalesModelCategoryRestriction> GetRestrictionsByProject(long projectId)
        {
            return _finder.Find(CategorySpecs.SalesModelCategoryRestrictions.Find.ByProject(projectId)).Many();
        }

        public IReadOnlyCollection<long> GetDependedByRestrictionsInProjectOrderIds(long projectId)
        {
            return _finder.Find(Specs.Find.ById<Project>(projectId))
                          .Map(q => q.Select(x => x.OrganizationUnit)
                                     .SelectMany(x => x.OrdersByDestination)
                                     .Where(Specs.Find.ActiveAndNotDeleted<Order>() &&
                                            OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved,
                                                                                OrderState.OnApproval,
                                                                                OrderState.OnRegistration,
                                                                                OrderState.OnTermination))
                                     .Select(x => x.Id))
                          .Many();
        }
    }
}