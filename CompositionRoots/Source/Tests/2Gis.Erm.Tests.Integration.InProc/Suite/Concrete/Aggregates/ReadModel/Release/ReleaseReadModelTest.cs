using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Release
{
    public class ReleaseReadModelTest : IIntegrationTest 
    {
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider;
        private readonly IAppropriateEntityProvider<ReleaseInfo> _releaseInfoProvider;
        private readonly IAppropriateEntityProvider<Order> _orderEntityProvider; 

        public ReleaseReadModelTest(IReleaseReadModel releaseReadModel,
                                    IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider,
                                    IAppropriateEntityProvider<ReleaseInfo> releaseInfoProvider, 
                                    IAppropriateEntityProvider<Order> orderEntityProvider)
        {
            _releaseReadModel = releaseReadModel;
            _organizationUnitProvider = organizationUnitProvider;
            _releaseInfoProvider = releaseInfoProvider;
            _orderEntityProvider = orderEntityProvider;
        }

        public ITestResult Execute()
        {
            var date = DateTime.Now.AddMonths(-1);
            var timePeriod = new TimePeriod(date.GetFirstDateOfMonth(), date.GetLastDateOfMonth());

            var orgUnitForRelease = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                  new FindSpecification<OrganizationUnit>(
                                                                      ou => ou.DgppId != null &&
                                                                      ou.OrdersByDestination.Any(
                                                                          o =>
                                                                          o.BeginDistributionDate <= timePeriod.Start &&
                                                                          o.BeginDistributionDate >= timePeriod.End &&
                                                                          (o.WorkflowStepId == OrderState.Approved ||
                                                                           o.WorkflowStepId == OrderState.OnTermination) &&
                                                                          o.LegalPersonId != null && o.IsActive && !o.IsDeleted)));

            var releaseInfo = _releaseInfoProvider.Get(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>() && ReleaseSpecs.Releases.Find.ForPeriod(timePeriod));

            var orders = _orderEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Order>(), 5);

            if (orgUnitForRelease == null || releaseInfo == null || !orders.Any())
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var lastFinalRelease = _releaseReadModel.GetLastFinalRelease(releaseInfo.OrganizationUnitId, timePeriod);
            var validationReportLines = _releaseReadModel.GetOrderValidationLines(orders.Select(x => x.Id).ToArray());
            // ReSharper disable once PossibleInvalidOperationException
            _releaseReadModel.GetOrganizationUnitId((int)orgUnitForRelease.DgppId);
            var organizationUnitName = _releaseReadModel.GetOrganizationUnitName(orgUnitForRelease.Id);
            var releaseInfoFromReadModel = _releaseReadModel.GetReleaseInfo(releaseInfo.Id);
            var releaseProcessingMessages = _releaseReadModel.GetReleaseValidationResults(releaseInfoFromReadModel.Id);
            _releaseReadModel.HasFinalReleaseAfterDate(orgUnitForRelease.Id, date);
            _releaseReadModel.HasFinalReleaseInProgress(orgUnitForRelease.Id, timePeriod);
            _releaseReadModel.HasSuccededFinalReleaseFromDate(orgUnitForRelease.Id, date);
            _releaseReadModel.IsReleaseMustBeLaunchedThroughExport(orgUnitForRelease.Id);

            return new object[]
                {
                    lastFinalRelease,
                    validationReportLines,
                    organizationUnitName,
                    releaseInfoFromReadModel,
                    releaseProcessingMessages
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}