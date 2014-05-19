﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Release
{
    public class ReleaseReadModelTest : IIntegrationTest 
    {
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider;
        private readonly IAppropriateEntityProvider<ReleaseInfo> _releaseInfoProvider;
        private readonly IAppropriateEntityProvider<Platform.Model.Entities.Erm.Order> _orderEntityProvider; 

        public ReleaseReadModelTest(IReleaseReadModel releaseReadModel,
                                    IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider,
                                    IAppropriateEntityProvider<ReleaseInfo> releaseInfoProvider, IAppropriateEntityProvider<Platform.Model.Entities.Erm.Order> orderEntityProvider)
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
                                                                          (o.WorkflowStepId == (int)OrderState.Approved ||
                                                                           o.WorkflowStepId == (int)OrderState.OnTermination) &&
                                                                          o.LegalPersonId != null && o.IsActive && !o.IsDeleted)));

            var releaseInfo = _releaseInfoProvider.Get(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>() && ReleaseSpecs.Releases.Find.ForPeriod(timePeriod));

            var orders = _orderEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Platform.Model.Entities.Erm.Order>(), 5);

            if (orgUnitForRelease == null || releaseInfo == null || !orders.Any())
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var lastRelease = _releaseReadModel.GetLastRelease(releaseInfo.OrganizationUnitId, timePeriod);
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
                    lastRelease,
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