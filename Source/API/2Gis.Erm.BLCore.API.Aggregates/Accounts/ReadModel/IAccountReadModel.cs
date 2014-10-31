using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
{
    public interface IAccountReadModel : IAggregateReadModel<Account>
    {
        bool TryGetLimitLockingRelease(Limit limit, out string name);
        IEnumerable<Limit> GetLimitsForRelease(long releasingOrganizationUnitId, TimePeriod period);
        IEnumerable<Limit> GetHungLimitsByOrganizationUnitForDate(long organizationUnitId, DateTime limitStart);
        IEnumerable<Limit> GetClosedLimits(long organizationUnitId, TimePeriod period);
        IEnumerable<LockDto> GetActiveLocksForDestinationOrganizationUnitByPeriod(long organizationUnitId, TimePeriod period);
        bool HasActiveLocksForSourceOrganizationUnitByPeriod(long organizationUnitId, TimePeriod period);
        bool HasInactiveLocksForDestinationOrganizationUnit(long organizationUnitId, TimePeriod period);

        long ResolveDebitForOrderPaymentOperationTypeId();
        WithdrawalInfo GetLastWithdrawal(long organizationUnitId, TimePeriod period);
        WithdrawalDto[] GetInfoForWithdrawal(long organizationUnitId, TimePeriod period);
        IReadOnlyCollection<WithdrawalInfoDto> GetBlockingWithdrawals(long destProjectId, TimePeriod period);
        RevertWithdrawalDto[] GetInfoForRevertWithdrawal(long organizationUnitId, TimePeriod period);
        BranchOfficeOrganizationUnit FindPrimaryBranchOfficeOrganizationUnit(long organizationUnitId);
        Account FindAccount(long legalPersonId, long branchOfficeOrganizationUnitId);
        string GetLegalPersonShortName(long legalPersonId);
        string GetBranchOfficeOrganizationUnitName(long branchOfficeOrganizationUnitId);
        IReadOnlyCollection<LockDto> GetLockDetailsWithPlannedProvision(long organizationUnitId, TimePeriod period);
        bool AnyLockDetailsCreated(Guid chargeSessionId);
        AccountIdAndOwnerCodeDto GetAccountIdAndOwnerCodeByOrder(long orderId);
        IEnumerable<AccountDetailInfoToSendNotificationDto> GetAccountDetailsInfoToSendNotification(IEnumerable<long> accountDetailIds);
        long GetAccountOwnerCode(long accountId);
        Limit GetLimitById(long id);
        Limit GetLimitByReplicationCode(Guid replicationCode);
        LimitDto InitializeLimitForAccount(long accountId);
        bool IsThereLimitDuplicate(long limitId, long accountId, DateTime periodStartDate, DateTime periodEndDate);
        bool IsLimitRecalculationAvailable(long limitId);
        decimal CalculateLimitValueForAccountByPeriod(long accountId, DateTime periodStart, DateTime periodEnd);
    }
}