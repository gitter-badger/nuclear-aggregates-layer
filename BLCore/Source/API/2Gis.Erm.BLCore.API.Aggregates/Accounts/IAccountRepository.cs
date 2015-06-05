using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts
{
    public interface IAccountRepository : IAggregateRootRepository<Account>,
                                          IDeleteAggregateRepository<OperationType>,
                                          IDeleteAggregateRepository<Lock>,
                                          IDeleteAggregateRepository<LockDetail>
    {
        IDictionary<long, decimal> GetBalanceByAccounts(IEnumerable<long> accountIds);
        AccountDetail GetAccountDetail(long entityId);
        OperationTypeDto GetOperationTypeDto(long entityId);
        GetLockDetailDto GetLockDetail(long entityId);

        bool IsActiveLocksExists(long orderId);
        bool IsNonDeletedLocksExists(long orderId);
        int GetNonDeletedLocksCount(long orderId);
        int GetActiveLocksCount(long orderId);
        int GetInactiveLocksCount(long orderId);
        void UpdateAccountBalance(IEnumerable<long> accountIds);
        decimal? CalculateDebitForOrderPaymentSum(long orderId);
        void RecalculateLockValue(Lock lockEntity);
        Account FindAccount(long entityId);
        Account FindAccount(long branchOfficeOrganizationUnitId, long legalPersonId);
        bool IsCreateAccountDetailValid(long accountId, long userCode, bool checkContributionType);

        AccountDetail[] GetAccountDetailsForImportFrom1COperation(string branchOfficeOrganizationUnit1CCode,
                                                                  DateTime transactionPeriodStart,
                                                                  DateTime transactionPeriodEnd);

        IEnumerable<Account> GetAccountsByLegalPerson(string legalPersonSyncCode1C);
        IEnumerable<AccountFor1CExportDto> GetAccountsForExortTo1C(long organizationUnitId);

        Account CreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId);
        Account SecureCreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId);
        int Create(OperationType operationType);
        int Create(AccountDetail accountDetail);
        int Create(IEnumerable<AccountDetail> accountDetails);
        int Create(Lock @lock);

        int Update(Account account);
        int Update(OperationType operationType);
        int Update(AccountDetail accountDetail);
        int Update(Lock @lock);
        int Update(LockDetail lockDetail);

        int Delete(Account account);
        int Delete(AccountDetail accountDetail);
        int Delete(IEnumerable<AccountDetail> accountDetails);
        int Delete(LockDetail entity);
        int Delete(Lock entity);
        int Delete(OperationType entity);

        IEnumerable<OperationType> GetOperationTypes(string syncCode1C);

        IEnumerable<AccountInfoForImportFrom1C> GetAccountsForImportFrom1C(IEnumerable<string> branchOfficeSyncCodes,
                                                                           DateTime transactionPeriodStart,
                                                                           DateTime transactionPeriodEnd);

        IReadOnlyCollection<OperationType> GetOperationsInSyncWith1C();
    }
}