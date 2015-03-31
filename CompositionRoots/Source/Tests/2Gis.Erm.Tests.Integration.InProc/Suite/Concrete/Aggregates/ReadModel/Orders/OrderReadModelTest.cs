using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Orders
{
    public class OrderReadModelTest : IIntegrationTest
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider;

        public OrderReadModelTest(IOrderReadModel orderReadModel, IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider)
        {
            _orderReadModel = orderReadModel;
            _organizationUnitProvider = organizationUnitProvider;
        }

        public ITestResult Execute()
        {
            var date = DateTime.Now.AddMonths(-1);
            var timePeriod = new TimePeriod(date.GetFirstDateOfMonth(), date.GetLastDateOfMonth());

            var orgUnitForRelease = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                  new FindSpecification<OrganizationUnit>(
                                                                      ou =>
                                                                      ou.OrdersByDestination.Any(
                                                                          o =>
                                                                          o.BeginDistributionDate <= timePeriod.Start &&
                                                                          o.BeginDistributionDate >= timePeriod.End &&
                                                                          (o.WorkflowStepId == OrderState.Approved ||
                                                                           o.WorkflowStepId == OrderState.OnTermination) &&
                                                                          o.LegalPersonId != null && o.IsActive && !o.IsDeleted)));

            if (orgUnitForRelease == null)
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var ordersForRelease = _orderReadModel.GetOrderIdsForRelease(orgUnitForRelease.Id, timePeriod);
            var orderReleaseInfos = _orderReadModel.GetOrderReleaseInfos(orgUnitForRelease.Id, timePeriod);
            var orderValidationAdditionalInfos = _orderReadModel.GetOrderValidationAdditionalInfos(ordersForRelease ?? new long[0]);
            var ordersCompletelyReleasedBySourceOrganizationUnit = _orderReadModel.GetOrdersCompletelyReleasedBySourceOrganizationUnit(orgUnitForRelease.Id);
            
            return new object[]
                {
                    orderReleaseInfos,
                    orderValidationAdditionalInfos,
                    ordersCompletelyReleasedBySourceOrganizationUnit,
                    ordersForRelease
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}