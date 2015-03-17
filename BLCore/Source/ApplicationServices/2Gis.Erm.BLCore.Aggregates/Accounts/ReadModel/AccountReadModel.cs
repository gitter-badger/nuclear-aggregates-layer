using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
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
        /// ¬ыбрать все лимиты (активные и т.п.), относ€щиес€ к указанной сборке, которые подлежат закрытию.
        /// »сключаем лимиты по лицевым счетам, дл€ которых есть заказы выход€щие в других отделени€х организации, по которым ещЄ
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
                                                   && x.Status == ReleaseStatus.Success),
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

            // —обираем все потенциально блокирующие сборки по данному лимиту
            var releaseInfos = 
                _finder.Find(ReleaseSpecs.Releases.Find.FinalForPeriodWithStatuses(
                                    checkingPeriod,
                                    ReleaseStatus.InProgressInternalProcessingStarted, 
                                    ReleaseStatus.InProgressWaitingExternalProcessing, 
                                    ReleaseStatus.Success));
           
            var checkingOrders = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>()
                                              && OrderSpecs.Orders.Find.ByPeriod(checkingPeriod)
                                              && OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved, OrderState.OnTermination, OrderState.Archive)
                                              && OrderSpecs.Orders.Find.ByAccountWithLegalPersonCorrectnessCheck(limit.AccountId));

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
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(period.Start, period.End))
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
                                    && Specs.Find.InactiveAndNotDeletedEntities<Lock>())
                          .Any();
        }

        public long ResolveDebitForOrderPaymentOperationTypeId()
        {
            const string OperationTypeDebitForOrderPaymentSyncCode1C = "11";

            return _finder.Find(Specs.Select.Id<OperationType>(),
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
                              WithdrawalStatus = x.LastWithdrawal.Status
                          })
                          .ToArray();
        }

        public WithdrawalInfo GetLastWithdrawalIncludingUndefinedAccountingMethod(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<WithdrawalInfo>()
                                && AccountSpecs.Withdrawals.Find.ByOrganization(organizationUnitId)
                                && AccountSpecs.Withdrawals.Find.ForPeriod(period)
                                && (AccountSpecs.Withdrawals.Find.ByAccoutingMethod(accountingMethod) || AccountSpecs.Withdrawals.Find.WithNoAccountingMethodSpecified()))
                          .OrderByDescending(x => x.StartDate)
                          .FirstOrDefault();
        }

        public WithdrawalDto[] GetInfoForWithdrawal(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(period.Start, period.End) &&
                                AccountSpecs.Locks.Find.ByAccountingMethod(accountingMethod))
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

        public RevertWithdrawalDto[] GetInfoForRevertWithdrawal(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            return _finder.Find(Specs.Find.NotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(period.Start, period.End) &&
                                AccountSpecs.Locks.Find.ByAccountingMethod(accountingMethod))
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

        public WithdrawalInfo GetLastWithdrawal(long organizationUnitId, TimePeriod period, AccountingMethod accountingMethod)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<WithdrawalInfo>()
                                && AccountSpecs.Withdrawals.Find.ByOrganization(organizationUnitId)
                                && AccountSpecs.Withdrawals.Find.ForPeriod(period)
                                && AccountSpecs.Withdrawals.Find.ByAccoutingMethod(accountingMethod))
                          .OrderByDescending(x => x.StartDate)
                          .FirstOrDefault();
        }

        public BranchOfficeOrganizationUnit FindPrimaryBranchOfficeOrganizationUnit(long organizationUnitId)
        {
            return _finder.FindOne(BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.PrimaryBranchOfficeOrganizationUnit() &&
                                BranchOfficeSpecs.BranchOfficeOrganizationUnits.Find.ByOrganizationUnit(organizationUnitId));
        }

        public Account FindAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Account>() &&
                                AccountSpecs.Accounts.Find.ForLegalPersons(legalPersonId, branchOfficeOrganizationUnitId))
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

        public IEnumerable<AccountDetailInfoToSendNotificationDto> GetAccountDetailsInfoToSendNotification(IEnumerable<long> accountDetailIds)
        {
            return _finder.Find(Specs.Find.ByIds<AccountDetail>(accountDetailIds))
                          .Select(x => new AccountDetailInfoToSendNotificationDto
                              {
                                  Amount = x.Amount,
                                  AccountId = x.AccountId,
                                  IsPlusOperation = x.OperationType.IsPlus,
                                  OperationName = x.OperationType.Name,
                                  TransactionDate = x.TransactionDate,
                                  BranchOfficeName = x.Account.BranchOfficeOrganizationUnit.ShortLegalName,
                                  LegalPersonName = x.Account.LegalPerson.LegalName,
                                  AccountOwnerCode = x.Account.OwnerCode
                              })
                          .ToArray();
        }

        public long GetAccountOwnerCode(long accountId)
        {
            return _finder.Find(Specs.Find.ById<Account>(accountId)).Select(x => x.OwnerCode).Single();
        }

        public AccountIdAndOwnerCodeDto GetAccountIdAndOwnerCodeByOrder(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => new AccountIdAndOwnerCodeDto { AccountId = x.Account.Id, OwnerCode = x.Account.OwnerCode })
                          .SingleOrDefault();
        }

        public IReadOnlyCollection<LockDto> GetLockDetailsWithPlannedProvision(long organizationUnitId, TimePeriod period)
        {
            var orderPositionsQuery = _finder.Find(Specs.Find.ActiveAndNotDeleted<OrderPosition>());
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.BySourceOrganizationUnit(organizationUnitId) &&
                                AccountSpecs.Locks.Find.ForPeriod(period.Start, period.End))
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
                                                                                        IsPlannedProvision =
                                                                                    SalesModelUtil.PlannedProvisionSalesModels.Contains(op.PricePosition.Position.SalesModel)
                                                           })
                                                 .Where(x => x.IsPlannedProvision)
                                                 .Select(x => x.LockDetail)
                              })
                          .Where(x => x.LockDetails.Any())
                          .Select(x => new LockDto { Lock = x.Lock, Details = x.LockDetails })
                          .ToArray();
        }

        public Limit GetLimitById(long id)
        {
            return _finder.FindOne(Specs.Find.ById<Limit>(id));
        }

        public Limit GetLimitByReplicationCode(Guid replicationCode)
        {
            return _finder.FindOne(Specs.Find.ByReplicationCode<Limit>(replicationCode));
        }

        public LimitDto InitializeLimitForAccount(long accountId)
        {
            return _finder.Find(Specs.Find.ById<Account>(accountId))
                          .Select(x => new LimitDto
                          {
                              LegalPersonId = x.LegalPerson.Id,
                              LegalPersonName = x.LegalPerson.LegalName,
                              BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                              BranchOfficeId = x.BranchOfficeOrganizationUnit.BranchOffice.Id,
                              BranchOfficeName = x.BranchOfficeOrganizationUnit.BranchOffice.Name,
                              LegalPersonOwnerId = x.LegalPerson.OwnerCode,
                          })
                          .Single();
        }

        public bool IsThereLimitDuplicate(long limitId, long accountId, DateTime periodStartDate, DateTime periodEndDate)
        {
            return _finder.Find(Specs.Find.ById<Account>(accountId))
                          .SelectMany(account => account.Limits)
                          .Where(limit => !limit.IsDeleted)
                          .Any(limit => limit.StartPeriodDate == periodStartDate && limit.EndPeriodDate == periodEndDate && limit.Id != limitId);
        }

        public bool IsLimitRecalculationAvailable(long limitId)
        {
            var limit = _finder.FindOne(Specs.Find.ById<Limit>(limitId));

            // заказы, по которым считаетс€ лимит плюс заказы в состо€нии "на расторжении"
            var orderOrganizationUnits = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>()
                                                      && OrderSpecs.Orders.Find.ByAccount(limit.AccountId)
                                                      && OrderSpecs.Orders.Find.ByPeriod(new TimePeriod(limit.StartPeriodDate, limit.EndPeriodDate))
                                                      && OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved, OrderState.OnTermination))
                                                .Select(order => order.DestOrganizationUnitId)
                                                .Distinct()
                                                .ToArray();

            var releaseOrganizationUnits = _finder.Find(Specs.Find.ActiveAndNotDeleted<ReleaseInfo>()
                                                        && ReleaseSpecs.Releases.Find.Final()
                                                        && ReleaseSpecs.Releases.Find.Succeeded()
                                                        && ReleaseSpecs.Releases.Find.ForPeriod(new TimePeriod(limit.StartPeriodDate, limit.EndPeriodDate)))
                                                  .Select(info => info.OrganizationUnitId)
                                                  .Distinct()
                                                  .ToArray();

            var existsOrderWithoutFinalRelease = orderOrganizationUnits.Except(releaseOrganizationUnits).Any();

            // если заказов нет вообще (а не только заказов, по которым не проведена сборка), то пересчЄт лимита возможен
            return !orderOrganizationUnits.Any() || existsOrderWithoutFinalRelease;
        }

        public decimal CalculateLimitValueForAccountByPeriod(long accountId, DateTime periodStart, DateTime periodEnd)
        {
            var lockSum = _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>()
                                       && AccountSpecs.Locks.Find.ByAccount(accountId)
                                       && AccountSpecs.Locks.Find.ForPreviousPeriods(periodStart, periodEnd))
                                 .Sum(@lock => (decimal?)@lock.PlannedAmount) ?? 0;

            var orderReleaseSum = _finder.Find(Specs.Find.ActiveAndNotDeleted<Order>()
                                               && OrderSpecs.Orders.Find.ByAccount(accountId)
                                               && OrderSpecs.Orders.Find.WithStatuses(OrderState.Approved))
                                         .SelectMany(order => order.OrderReleaseTotals)
                                         .Where(OrderSpecs.OrderReleaseTotals.Find.ByPeriod(periodStart, periodEnd))
                                         .Sum(total => (decimal?)total.AmountToWithdraw) ?? 0;

            var accountBalanceRaw = _finder.Find(Specs.Find.ById<Account>(accountId)).Select(x => x.Balance).Single();
            var accountBalance = accountBalanceRaw - (lockSum + orderReleaseSum);

            return accountBalance > 0 ? 0 : Math.Abs(accountBalance);
        }

        public decimal CalculateLimitIncreasingValue(long limitId)
        {
            var limitInfo = _finder.Find(Specs.Find.ById<Limit>(limitId)).Select(x => new { x.AccountId, x.StartPeriodDate, x.EndPeriodDate, x.Amount }).Single();
            var newLimitAmount = CalculateLimitValueForAccountByPeriod(limitInfo.AccountId, limitInfo.StartPeriodDate, limitInfo.EndPeriodDate);
            var difference = newLimitAmount - limitInfo.Amount;

            return difference > 0 ? difference : 0;
        }

        public long GetLimitOwnerCode(long limitId)
        {
            return _finder.Find(Specs.Find.ById<Limit>(limitId)).Select(x => x.OwnerCode).Single();
        }

        public IDictionary<long, IEnumerable<AccountDetailForExportDto>> GetAccountDetailsForExportTo1C(IEnumerable<long> organizationUnitIds,
                                                                                                        DateTime periodStartDate,
                                                                                                        DateTime periodEndDate)
        {
            return
                _finder.Find(AccountSpecs.Locks.Find.BySourceOrganizationUnits(organizationUnitIds) &&
                             AccountSpecs.Locks.Find.ForPeriod(periodStartDate, periodEndDate) &&
                             Specs.Find.NotDeleted<Lock>() &&
                             Specs.Find.InactiveEntities<Lock>())
                       .Select(x => new AccountDetailForExportDto
                                        {
                                            OrganizationUnitSyncCode1C = x.Order.SourceOrganizationUnit.SyncCode1C,
                                            BranchOfficeOrganizationUnitSyncCode1C = x.Order.BranchOfficeOrganizationUnit.SyncCode1C,
                                            AccountCode = x.AccountId,
                                            SourceOrganizationUnitId = x.Order.SourceOrganizationUnitId,
                                            OrderNumber = x.Order.Number,
                                            OrderType = x.Order.OrderType,
                                            OrderSignupDateUtc = x.Order.SignupDate,
                                            DebitAccountDetailAmount = x.AccountDetail.Amount,
                                            ElectronicMedia = x.Order.DestOrganizationUnit.ElectronicMedia,
                                            OrderId = x.OrderId,
                                            ProfileCode = x.Order.LegalPersonProfileId != null
                                                              ? x.Order.LegalPersonProfile.Id
                                                              : x.Account.LegalPerson.LegalPersonProfiles
                                                                 .Where(p => !p.IsDeleted && p.IsMainProfile)
                                                                 .Select(p => p.Id)
                                                                 .FirstOrDefault(),
                                        })
                       .Where(x => x.DebitAccountDetailAmount > 0)
                       .GroupBy(x => x.SourceOrganizationUnitId)
                       .ToDictionary(x => x.Key, y => y.AsEnumerable());
        }

        public IEnumerable<long> GetOrganizationUnitsToProccessWithdrawals(DateTime periodStartDate, DateTime periodEndDate, AccountingMethod accountingMethod)
        {
            return _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                AccountSpecs.Locks.Find.ForPeriod(periodStartDate, periodEndDate) &&
                                AccountSpecs.Locks.Find.ByAccountingMethod(accountingMethod))
                          .Select(l => l.Order.SourceOrganizationUnitId)
                          .Distinct()
                          .ToArray();
        }
    }
}