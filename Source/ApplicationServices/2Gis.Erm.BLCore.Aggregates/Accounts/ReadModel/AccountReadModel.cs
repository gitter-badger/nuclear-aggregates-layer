using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel
{
    public sealed class AccountReadModel : IAccountReadModel
    {
        private readonly IFinder _finder;

        public AccountReadModel(IFinder finder)
        {
            _finder = finder;
        }

        /// <summary>
        /// Выбрать все лимиты (активные и т.п.), относящиеся к указанной сборке, которые подлежат закрытию.
        /// Исключаем лимиты по лицевым счетам, для которых есть заказы выходящие в других отделениях организации, по которым ещё
        /// не было финальной сборки за период
        /// </summary>
        public IEnumerable<Limit> GetLimitsForRelease(long releasingOrganizationUnitId, TimePeriod period)
        {
            var ordersForCurrentRelease =
                _finder.Find(OrderSpecs.Orders.Find.ForRelease(releasingOrganizationUnitId, period)
                                && Specs.Find.ActiveAndNotDeleted<Order>()
                                && OrderSpecs.Orders.Find.HasLegalPerson())
                       .SelectMany(o => o.BranchOfficeOrganizationUnit.Accounts.Where(a => a.IsActive
                                                                                           && !a.IsDeleted
                                                                                           && a.LegalPersonId == o.LegalPersonId))
                       .GroupBy(account => account.Id)
                       .Select(x => x.FirstOrDefault());

            var ordersForReleasesByOtherOrganizationUnits =
                _finder.Find(OrderSpecs.Orders.Find.AllForReleaseByPeriodExceptOrganizationUnit(releasingOrganizationUnitId, period)
                               && Specs.Find.ActiveAndNotDeleted<Order>()
                               && OrderSpecs.Orders.Find.HasLegalPerson())
                       .Select(o => new
                           {
                               HasFinalRelease =
                                        o.DestOrganizationUnit
                                         .ReleaseInfos
                                         .Any(x => x.IsActive
                                                   && !x.IsDeleted
                                                   && !x.IsBeta
                                                   && x.PeriodStartDate >= period.Start && x.PeriodEndDate <= period.End
                                                   && x.OrganizationUnitId == o.DestOrganizationUnitId
                                                   && x.Status == (int)ReleaseStatus.Success),
                               o.AccountId
                           });

            return ordersForCurrentRelease
                    .GroupJoin(ordersForReleasesByOtherOrganizationUnits,
                               account => account.Id,
                               x => x.AccountId,
                               (account, enumerable) =>
                               new
                               {
                                   account.Limits,
                                   ExistNotReleasedOrdersByAccount = enumerable.Any(x => !x.HasFinalRelease)
                               })
                    .Where(x => !x.ExistNotReleasedOrdersByAccount)
                    .SelectMany(x => x.Limits)
                    .Where(Specs.Find.ActiveAndNotDeleted<Limit>()
                            && AccountSpecs.Limits.Find.ApprovedForPeriod(period))
                    .ToArray();
        }

        public bool TryGetLimitLockingRelease(Limit limit, out string name)
        {
            name = null;

            var checkingPeriod = new TimePeriod(limit.StartPeriodDate, limit.EndPeriodDate);
            // Собираем все потенциально блокирующие сборки по данному лимиту
            var releaseInfos = 
                _finder.Find(ReleaseSpecs.Releases.Find.FinalForPeriodWithStatuses(
                                    checkingPeriod,
                                    ReleaseStatus.InProgressInternalProcessingStarted, 
                                    ReleaseStatus.InProgressWaitingExternalProcessing, 
                                    ReleaseStatus.Success));
           
            var checkingOrders = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>()
                                              && OrderSpecs.Orders.Find.ByPeriod(checkingPeriod)
                                              && OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved, OrderState.OnTermination, OrderState.Archive)
                                              && OrderSpecs.Orders.Find.ByAccount(limit.AccountId));

            var blockingReleases = 
                    checkingOrders
                        .Join(releaseInfos,
                              order => order.DestOrganizationUnitId,
                              relInfo => relInfo.OrganizationUnitId,
                              (order, relInfo) => new { relInfo.Id, relInfo.Status, relInfo.IsBeta, relInfo.StartDate, relInfo.OrganizationUnit.Name })
                        .OrderByDescending(x => x.StartDate)
                        .FirstOrDefault();
            if (blockingReleases == null)
            {
                return false;
            }
            
            name = blockingReleases.Name;
            return true;
        }

        public IEnumerable<Limit> GetHungLimitsByOrganizationUnitForDate(long organizationUnitId, DateTime limitStart)
        {
            return _finder.Find<Limit>(limit => limit.StartPeriodDate <= limitStart &&
                                                limit.Account.BranchOfficeOrganizationUnit.OrganizationUnitId == organizationUnitId &&
                                                limit.IsActive &&
                                                !limit.IsDeleted)
                          .ToArray();
        }

        public IEnumerable<Limit> GetClosedLimits(long destinationOrganizationUnitId, TimePeriod period)
        {
            return (from @lock in _finder.Find(AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(destinationOrganizationUnitId, period))
                    from account in @lock.Order.BranchOfficeOrganizationUnit.Accounts
                        .Where(a => a.IsActive
                                    && !a.IsDeleted
                                    && a.LegalPersonId == @lock.Order.LegalPersonId)
                    from limit in account.Limits
                    where !limit.IsActive
                        && !limit.IsDeleted
                        && limit.StartPeriodDate == period.Start
                        && limit.EndPeriodDate == period.End
                    select limit).Distinct().ToArray();
        }

        public IEnumerable<LockDto> GetActiveLocksForDestinationOrganizationUnitByPeriod(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>()
                                    && AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId, period))
                   .Select(l =>
                       new LockDto
                           {
                               Lock = l, 
                               Details = l.LockDetails
                           });
        }

        public bool HasActiveLocksForSourceOrganizationUnitByPeriod(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>()
                                    && AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId, period))
                   .Select(l =>
                       new LockDto
                       {
                           Lock = l,
                           Details = l.LockDetails
                       })
                   .Any();
        }

        public bool HasInactiveLocksForDestinationOrganizationUnit(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId, period)
                                    && Specs.Find.InactiveEntities<Lock>())
                          .Any();
        }

        public long ResolveDebitForOrderPaymentOperationTypeId()
        {
            const string OperationTypeDebitForOrderPaymentSyncCode1C = "11";

            return _finder
                        .Find<OperationType, long>(
                            Specs.Select.Id<OperationType>(), 
                            AccountSpecs.OperationTypes.Find.BySyncCode1C(OperationTypeDebitForOrderPaymentSyncCode1C))
                        .Single();
        }

        public IReadOnlyCollection<WithdrawalInfoDto> GetBlockingWithdrawals(long destProjectId, TimePeriod period)
        {
            var organizationUnitId = _finder.Find(Specs.Find.ById<Project>(destProjectId)).Select(x => x.OrganizationUnitId).SingleOrDefault();
            if (organizationUnitId == null)
            {
                return new WithdrawalInfoDto[0];
            }

            var withdrawalInfosQuery = _finder.Find(AccountSpecs.Withdrawals.Find.ForPeriod(period) &&
                                                    AccountSpecs.Withdrawals.Find.InStates(WithdrawalStatus.InProgress,
                                                                                           WithdrawalStatus.Withdrawing,
                                                                                           WithdrawalStatus.Reverting));

            return _finder.Find(Specs.Find.NotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId.Value, period))
                          .Select(x => x.Order.SourceOrganizationUnit)
                          .GroupJoin(withdrawalInfosQuery,
                                     ou => ou.Id,
                                     wi => wi.OrganizationUnitId,
                                     (ou, wi) => new
                                     {
                                         OrganizationUnit = ou,
                                         LastWithdrawal = wi.OrderByDescending(x => x.StartDate).FirstOrDefault()
                                     })
                          .Where(x => x.LastWithdrawal != null)
                          .Select(x => new WithdrawalInfoDto
                          {
                              OrganizationUnitId = x.OrganizationUnit.Id,
                              OrganizationUnitName = x.OrganizationUnit.Name,
                              WithdrawalStatus = (WithdrawalStatus)x.LastWithdrawal.Status
                          })
                          .ToArray();
        }

        public WithdrawalDto[] GetInfoForWithdrawal(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                              AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId, period))
                                   .Select(x => new
                                       {
                                            Lock = x,
                                            LockDetails = x.LockDetails.Where(y => !y.IsDeleted && y.IsActive),
                                            CalculatedLockBalance = x.LockDetails.Where(y => !y.IsDeleted && y.IsActive).Sum(y => (decimal?)y.Amount) ?? 0M,
                                            x.Account,
                                            AccountBalanceBeforeWithdrawal = x.Account.AccountDetails
                                                                            .Where(y => !y.IsDeleted && y.IsActive)
                                                                            .Sum(y => (decimal?)(y.OperationType.IsPlus ? y.Amount : -y.Amount)) ?? 0M,
                                            x.Order,
                                            TargetWithdrawalReleaseNumber = x.Order.BeginReleaseNumber + x.Order.Locks.Count(l => !l.IsActive && !l.IsDeleted) + 1,
                                       })
                                   .Select(x => new WithdrawalDto
                                   {
                                       Lock = x.Lock,
                                       LockDetails = x.LockDetails,
                                       CalculatedLockBalance = x.CalculatedLockBalance,
                                       Account = x.Account,
                                       AccountBalanceBeforeWithdrawal = x.AccountBalanceBeforeWithdrawal,
                                       Order = x.Order,
                                       AmountAlreadyWithdrawnAfterWithdrawal =
                                            (x.Order.Locks
                                                .Where(l => !l.IsActive && !l.IsDeleted && l.DebitAccountDetailId != null)
                                                .Sum(l => (decimal?)l.AccountDetail.Amount) ?? 0M) + x.CalculatedLockBalance,
                                       AmountToWithdrawNextAfterWithdrawal =
                                            x.Order.OrderReleaseTotals.Where(ort => ort.ReleaseNumber == x.TargetWithdrawalReleaseNumber)
                                                .Sum(ort => (decimal?)ort.AmountToWithdraw) ?? 0M
                                   })
                                   .ToArray();
        }

        public RevertWithdrawalDto[] GetInfoForRevertWithdrawal(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.NotDeleted<Lock>() &&
                              AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId, period))
                                   .Select(x => new
                                   {
                                       Lock = x,
                                       LockDetails = x.LockDetails.Where(y => !y.IsDeleted && !y.IsActive),
                                       CalculatedLockBalance = x.LockDetails.Where(y => !y.IsDeleted && !y.IsActive).Sum(y => (decimal?)y.Amount) ?? 0M,
                                       x.Account,
                                       DebitAccountDetail = x.AccountDetail,
                                       AccountBalanceBeforeRevertWithdrawal = x.Account.AccountDetails
                                                                       .Where(y => !y.IsDeleted && y.IsActive)
                                                                       .Sum(y => (decimal?)(y.OperationType.IsPlus ? y.Amount : -y.Amount)) ?? 0M,
                                       x.Order,
                                       TargetWithdrawalReleaseNumber = x.Order.BeginReleaseNumber + x.Order.Locks.Count(l => !l.IsActive && !l.IsDeleted) - 1,
                                   })
                                   .Select(x => new RevertWithdrawalDto
                                   {
                                       Account = x.Account,
                                       DebitAccountDetail = x.DebitAccountDetail,
                                       AccountBalanceBeforeRevertWithdrawal = x.AccountBalanceBeforeRevertWithdrawal,
                                       Lock = x.Lock,
                                       LockDetails = x.LockDetails,
                                       Order = x.Order,
                                       AmountAlreadyWithdrawnAfterWithdrawalRevert =
                                            (x.Order.Locks
                                                .Where(l => !l.IsActive && !l.IsDeleted && l.DebitAccountDetailId != null)
                                                .Sum(l => (decimal?)l.AccountDetail.Amount) ?? 0M) - x.CalculatedLockBalance,
                                       AmountToWithdrawNextAfterWithdrawalRevert =
                                          x.Order.OrderReleaseTotals
                                                .Where(ort => ort.ReleaseNumber == x.TargetWithdrawalReleaseNumber)
                                                .Sum(ort => (decimal?)ort.AmountToWithdraw) ?? 0M
                                   })
                                   .ToArray();
        }

        public WithdrawalInfo GetLastWithdrawal(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<WithdrawalInfo>()
                                && AccountSpecs.Withdrawals.Find.ByOrganization(organizationUnitId)
                                && AccountSpecs.Withdrawals.Find.ForPeriod(period))
                          .OrderByDescending(x => x.StartDate)
                          .FirstOrDefault();
        }

        public BranchOfficeOrganizationUnit FindPrimaryBranchOfficeOrganizationUnit(long organizationUnitId)
        {
            return _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryBranchOfficeOrganizationUnit() &&
                                BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.BelongsToOrganizationUnit(organizationUnitId));
        }

        public Account FindAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            return _finder.Find(AccountSpecs.Accounts.Find.ForLegalPersons(legalPersonId, branchOfficeOrganizationUnitId))
                          .FirstOrDefault();
        }

        public string GetLegalPersonShortName(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                          .Select(person => person.ShortName)
                          .SingleOrDefault();
        }

        public string GetBranchOfficeOrganizationUnitName(long branchOfficeOrganizationUnitId)
        {
            return _finder.Find(Specs.Find.ById<BranchOfficeOrganizationUnit>(branchOfficeOrganizationUnitId))
                          .Select(unit => unit.ShortLegalName)
                          .SingleOrDefault();
        }

        public bool AnyLockDetailsCreated(Guid chargeSessionId)
        {
            return _finder.Find(Specs.Find.NotDeleted<LockDetail>() && AccountSpecs.LockDetails.Find.ForChargeSessionId(chargeSessionId)).Any();
        }

        public IReadOnlyCollection<LockDto> GetLockDetailsWithPlannedProvision(long organizationUnitId, TimePeriod period)
        {
            var orderPositionsQuery = _finder.Find(Specs.Find.ActiveAndNotDeleted<OrderPosition>());
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() && AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId, period))
                          .Select(l => new
                              {
                                  Lock = l,
                                  LockDetails = l.LockDetails
                                                 .Where(ld => ld.IsActive && !ld.IsDeleted)
                                                 .Join(orderPositionsQuery,
                                                       ld => ld.OrderPositionId,
                                                       op => op.Id,
                                                       (ld, op) => new
                                                           {
                                                               LockDetail = ld,
                                                               IsPlannedProvision = op.PricePosition.Position.AccountingMethodEnum ==
                                                                                    (int)PositionAccountingMethod.PlannedProvision
                                                           })
                                                 .Where(x => x.IsPlannedProvision)
                                                 .Select(x => x.LockDetail)
                              })
                          .Where(x => x.LockDetails.Any())
                          .Select(x => new LockDto { Lock = x.Lock, Details = x.LockDetails })
                          .ToArray();
        }
    }
}