using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Accounts
{
    public class AccountReadModelTest : IIntegrationTest
    {
        private readonly IAccountReadModel _accountReadModel;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitProvider;
        private readonly IAppropriateEntityProvider<Order> _orderProvider;
        private readonly IAppropriateEntityProvider<Lock> _lockProvider;
        private readonly IAppropriateEntityProvider<Limit> _limitProvider;
        private readonly IAppropriateEntityProvider<WithdrawalInfo> _withdrawalInfoProvider;

        public AccountReadModelTest(IAccountReadModel accountReadModel, 
            IAppropriateEntityProvider<OrganizationUnit> organizationUnitProvider,
            IAppropriateEntityProvider<Order> orderProvider,
            IAppropriateEntityProvider<Lock> lockProvider, 
            IAppropriateEntityProvider<Limit> limitProvider,
            IAppropriateEntityProvider<WithdrawalInfo> withdrawalInfoProvider)
        {
            _accountReadModel = accountReadModel;
            _organizationUnitProvider = organizationUnitProvider;
            _orderProvider = orderProvider;
            _lockProvider = lockProvider;
            _limitProvider = limitProvider;
            _withdrawalInfoProvider = withdrawalInfoProvider;
        }

        public ITestResult Execute()
        {
            var activeLock = _lockProvider.Get(Specs.Find.ActiveAndNotDeleted<Lock>());
            var activeLockOrder = _orderProvider.Get(Specs.Find.ById<Order>(activeLock.OrderId));
            var activeLockTimePeriod = new TimePeriod(activeLock.PeriodStartDate, activeLock.PeriodEndDate);

            var dateForHungLimits = DateTime.UtcNow.AddMonths(-2);
            var orgUnitWithHungLimits = GetOrgUnitWithHungLimits(dateForHungLimits);

            var closedLimit = _limitProvider.Get(Specs.Find.InactiveAndNotDeletedEntities<Limit>());
            var orgUnitWithClosedLimit = GetOrgUnitWithClosedLimit(closedLimit);
            var closedLimitTimePeriod = new TimePeriod(closedLimit.StartPeriodDate, closedLimit.EndPeriodDate);

            var limitForRelease = GetLimitForRelease();
            var limitForReleaseTimePeriod = new TimePeriod(limitForRelease.StartPeriodDate, limitForRelease.EndPeriodDate);
            var orderWithLimitsForRelease = GetOrderWithLimitsForRelease(limitForRelease);

            var withdrawal = _withdrawalInfoProvider.Get(Specs.Find.ActiveAndNotDeleted<WithdrawalInfo>());
            var withdrawalTimePeriod = new TimePeriod(withdrawal.PeriodStartDate, withdrawal.PeriodEndDate);
            var orgUnitWithWithdrawal = GetOrgUnitWithWithdrawal(withdrawal);

            try
            {
                _accountReadModel
                    .GetActiveLocksForDestinationOrganizationUnitByPeriod(activeLockOrder.DestOrganizationUnitId, activeLockTimePeriod)
                    .Should().NotBeEmpty();

                _accountReadModel
                    .GetInfoForRevertWithdrawal(activeLockOrder.SourceOrganizationUnitId, activeLockTimePeriod, AccountingMethod.GuaranteedProvision)
                    .Should().NotBeEmpty();

                _accountReadModel
                    .GetInfoForRevertWithdrawal(activeLockOrder.SourceOrganizationUnitId, activeLockTimePeriod, AccountingMethod.PlannedProvision)
                    .Should().NotBeEmpty();

                _accountReadModel
                    .GetInfoForWithdrawal(activeLockOrder.SourceOrganizationUnitId, activeLockTimePeriod, AccountingMethod.PlannedProvision)
                    .Should().NotBeEmpty();

                _accountReadModel.GetHungLimitsByOrganizationUnitForDate(orgUnitWithHungLimits.Id, dateForHungLimits)
                    .Should().NotBeEmpty();

                _accountReadModel.GetClosedLimits(orgUnitWithClosedLimit.Id, closedLimitTimePeriod)
                    .Should().NotBeEmpty();

                // TODO {all, 06.02.2014}: тест не проходит по таймауту, стоит оптимизировать запрос
//                _accountReadModel.GetLimitsForRelease(orderWithLimitsForRelease.DestOrganizationUnitId, limitForReleaseTimePeriod)
//                    .Should().NotBeEmpty();

                _accountReadModel.GetLastWithdrawal(orgUnitWithWithdrawal.Id, withdrawalTimePeriod, AccountingMethod.GuaranteedProvision)
                    .Should().NotBeNull();

                _accountReadModel.GetLastWithdrawalIncludingUndefinedAccountingMethod(orgUnitWithWithdrawal.Id, withdrawalTimePeriod, AccountingMethod.GuaranteedProvision)
                  .Should().NotBeNull();

                return OrdinaryTestResult.As.Succeeded;
            }
            catch (Exception ex)
            {
                return OrdinaryTestResult.As.Asserted(ex);
            }
        }

        private Limit GetLimitForRelease()
        {
            return _limitProvider.Get(Specs.Find.ActiveAndNotDeleted<Limit>() 
                && new FindSpecification<Limit>(x => x.Status == LimitStatus.Approved
                    && x.Account.Orders
                        .Any(o => (o.WorkflowStepId == OrderState.Approved || o.WorkflowStepId == OrderState.OnTermination)
                            && o.IsActive && !o.IsDeleted && o.LegalPersonId != null)));
        }

        private Order GetOrderWithLimitsForRelease(Limit limitForRelease)
        {
            return _orderProvider.Get(Specs.Find.ActiveAndNotDeleted<Order>()
                && new FindSpecification<Order>(o => o.Account.Limits.Any(l => l.Id == limitForRelease.Id)));
        }

        private OrganizationUnit GetOrgUnitWithWithdrawal(WithdrawalInfo withdrawal)
        {
            return _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>()
                && new FindSpecification<OrganizationUnit>(ou => ou.WithdrawalInfos.Any(w => w.Id == withdrawal.Id)));
        }

        private OrganizationUnit GetOrgUnitWithClosedLimit(Limit closedLimit)
        {
            return _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>()
                 && new FindSpecification<OrganizationUnit>(ou => ou.OrdersByDestination
                    .Any(o => o.BranchOfficeOrganizationUnit.Accounts
                        .Any(a => a.IsActive && !a.IsDeleted && a.LegalPersonId == o.LegalPersonId 
                            && a.Limits.Any(l => l.Id == closedLimit.Id)))));
        }

        private OrganizationUnit GetOrgUnitWithHungLimits(DateTime dateForHungLimits)
        {
            return _organizationUnitProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>()
                && new FindSpecification<OrganizationUnit>(ou => ou.BranchOfficeOrganizationUnits
                        .Any(boou => boou.Accounts
                            .Any(a => a.Limits
                                .Any(l => l.IsActive && !l.IsDeleted && l.StartPeriodDate <= dateForHungLimits)))));
        }
    }
}