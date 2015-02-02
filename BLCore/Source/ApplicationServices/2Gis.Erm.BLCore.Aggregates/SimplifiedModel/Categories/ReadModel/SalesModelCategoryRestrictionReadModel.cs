using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.Categories.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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
            return _finder.FindMany(CategorySpecs.SalesModelCategoryRestrictions.Find.ByProject(projectId));
        }

        public IReadOnlyCollection<long> GetDependedByRestrictionsInProjectOrderIds(long projectId)
        {
            return _finder.Find(Specs.Find.ById<Project>(projectId))
                          .Select(x => x.OrganizationUnit)
                          .SelectMany(x => x.OrdersByDestination)
                          .Where(Specs.Find.ActiveAndNotDeleted<Order>() &&
                                 OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved,
                                                                     OrderState.OnApproval,
                                                                     OrderState.OnRegistration,
                                                                     OrderState.OnTermination))
                          .Select(x => x.Id)
                          .ToArray();
        }
    }
}