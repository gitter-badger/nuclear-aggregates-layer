using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Account
{
    public class AccountReadModelTest : IIntegrationTest
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider; 

        public AccountReadModelTest(IAccountReadModel accountReadModel, IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider)
        {
            _accountReadModel = accountReadModel;
            _organizationUnitProvider = organizationUnitProvider;
        }

        public ITestResult Execute()
        {
            var date = DateTime.Now.AddMonths(-1);
            var timePeriod = new TimePeriod(date.GetFirstDateOfMonth(), date.GetLastDateOfMonth());
            var dateForHungLImits = DateTime.UtcNow.AddMonths(-2);

            var orgUnitWithActiveLocks = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                       new FindSpecification<OrganizationUnit>(
                                                                           x => x.OrdersByDestination.Any(y => y.Locks.Any(z =>
                                                                                                                           z.IsActive && !z.IsDeleted &&
                                                                                                                           z.PeriodStartDate == timePeriod.Start &&
                                                                                                                           z.PeriodEndDate == timePeriod.End))));

            var orgUnitWithClosedLimits = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                        new FindSpecification<OrganizationUnit>(ou => ou.OrdersByDestination.Any(
                                                                            o =>
                                                                            o.BranchOfficeOrganizationUnit.Accounts.Any(
                                                                                a =>
                                                                                a.IsActive && !a.IsDeleted && a.LegalPersonId == o.LegalPersonId &&
                                                                                a.Limits.Any(
                                                                                    l =>
                                                                                    !l.IsActive && !l.IsDeleted && l.StartPeriodDate == timePeriod.Start &&
                                                                                    l.EndPeriodDate == timePeriod.End)))));
            


            var orgUnitWithHungLimits = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                      new FindSpecification<OrganizationUnit>(
                                                                          ou =>
                                                                          ou.BranchOfficeOrganizationUnits.Any(
                                                                              boou =>
                                                                              boou.Accounts.Any(
                                                                                  a =>
                                                                                  a.Limits.Any(
                                                                                      l => l.IsActive && !l.IsDeleted && l.StartPeriodDate <= dateForHungLImits)))));

            var orgUnitForRevertWithdrawal = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                           new FindSpecification<OrganizationUnit>(
                                                                               ou =>
                                                                               ou.OrdersBySource.Any(
                                                                                   o =>
                                                                                   o.Locks.Any(
                                                                                       l =>
                                                                                       !l.IsDeleted && l.PeriodStartDate == timePeriod.Start ||
                                                                                       l.PeriodEndDate == timePeriod.End))));

            var orgUnitForWithdrawal = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                     new FindSpecification<OrganizationUnit>(
                                                                         ou =>
                                                                         ou.OrdersBySource.Any(
                                                                             o =>
                                                                             o.Locks.Any(
                                                                                 l =>
                                                                                 l.IsActive && !l.IsDeleted && l.PeriodStartDate == timePeriod.Start ||
                                                                                 l.PeriodEndDate == timePeriod.End))));

            var orgUnitWithAnyWithdrawals = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                          new FindSpecification<OrganizationUnit>(
                                                                              ou =>
                                                                              ou.WithdrawalInfos.Any(
                                                                                  wi =>
                                                                                  wi.IsActive && !wi.IsDeleted && wi.PeriodStartDate == timePeriod.Start &&
                                                                                  wi.PeriodEndDate == timePeriod.End)));

            var orgUnitWithLimitsForRelease = _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>() &&
                                                                            new FindSpecification<OrganizationUnit>(
                                                                                ou =>
                                                                                ou.OrdersByDestination.Any(
                                                                                    o =>
                                                                                    o.BeginDistributionDate <= timePeriod.Start &&
                                                                                    o.BeginDistributionDate >= timePeriod.End &&
                                                                                    (o.WorkflowStepId == (int)OrderState.Approved ||
                                                                                     o.WorkflowStepId == (int)OrderState.OnTermination) &&
                                                                                    o.LegalPersonId != null && o.IsActive && !o.IsDeleted
                                                                                    && o.Account.Limits.Any(l => l.IsActive && !l.IsDeleted))));

            if (new object[]
                {
                    orgUnitWithActiveLocks,
                    orgUnitWithClosedLimits,
                    orgUnitWithHungLimits,
                    orgUnitForRevertWithdrawal,
                    orgUnitWithAnyWithdrawals,
                    orgUnitWithLimitsForRelease
                }.Any(x => x == null))
            {
                return OrdinaryTestResult.As.NotExecuted;
            }

            var activeLocksForDestinationOrganizationUnitByPeriod =
                _accountReadModel.GetActiveLocksForDestinationOrganizationUnitByPeriod(orgUnitWithActiveLocks.Id, timePeriod);

            var closedLimits = _accountReadModel.GetClosedLimits(orgUnitWithClosedLimits.Id, timePeriod);

            var hungLimitsByOrganizationUnitForDate = _accountReadModel.GetHungLimitsByOrganizationUnitForDate(orgUnitWithHungLimits.Id, dateForHungLImits);

            var infoForRevertWithdrawal = _accountReadModel.GetInfoForRevertWithdrawal(orgUnitForRevertWithdrawal.Id, timePeriod);

            var infoForWithdrawal = _accountReadModel.GetInfoForWithdrawal(orgUnitForWithdrawal.Id, timePeriod);

            var lastWithdrawal = _accountReadModel.GetLastWithdrawal(orgUnitWithAnyWithdrawals.Id, timePeriod);

            var limitsForRelease = _accountReadModel.GetLimitsForRelease(orgUnitWithLimitsForRelease.Id, timePeriod);

            return new object[]
                {
                    activeLocksForDestinationOrganizationUnitByPeriod,
                    closedLimits,
                    hungLimitsByOrganizationUnitForDate,
                    infoForRevertWithdrawal,
                    infoForWithdrawal,
                    lastWithdrawal,
                    limitsForRelease
                }.Any(x => x == null)
                       ? OrdinaryTestResult.As.Failed
                       : OrdinaryTestResult.As.Succeeded;
        }
    }
}