using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Import
{
    /// <summary>
    /// Приходится фэйкать, т.к. Moq не справляется с таким сложным интерфейсом - кидает эксепшн "Duplicate element: Castle.DynamicProxy.Generators.MetaMethod"
    /// </summary>
    internal class FakeAccountRepository : IAccountRepository
    {
        public FakeAccountRepository()
        {
            UpdatedAccountIds = new List<long>();
            OperationsInSyncWith1C = new List<OperationType>();
            CreatedAccountDetails = new List<AccountDetail>();
            DeletedAccountDetails = new List<AccountDetail>();
            AccountInfos = new List<AccountInfoForImportFrom1C>();
        }

        public List<long> UpdatedAccountIds { get; private set; }
        public List<OperationType> OperationsInSyncWith1C { get; private set; }
        public List<AccountDetail> CreatedAccountDetails { get; private set; }
        public List<AccountDetail> DeletedAccountDetails { get; private set; }
        public List<AccountInfoForImportFrom1C> AccountInfos { get; private set; }

        public int Activate(long entityId)
        {
            throw new NotImplementedException();
        }

        public int Deactivate(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeleteAggregateRepository<OperationType>.Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeleteAggregateRepository<Lock>.Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        int IDeleteAggregateRepository<LockDetail>.Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public void CheckForDebts(long entityId, long currentUserCode, bool bypassValidation)
        {
            throw new NotImplementedException();
        }

        public IDictionary<long, decimal> GetBalanceByAccounts(IEnumerable<long> accountIds)
        {
            throw new NotImplementedException();
        }

        public AccountDetail GetAccountDetail(long entityId)
        {
            throw new NotImplementedException();
        }

        public OperationTypeDto GetOperationTypeDto(long entityId)
        {
            throw new NotImplementedException();
        }

        public IDictionary<long, int> GetInactiveLocksCountByOrder(IEnumerable<long> orderIds)
        {
            throw new NotImplementedException();
        }

        public GetLockDetailDto GetLockDetail(long entityId)
        {
            throw new NotImplementedException();
        }

        public bool IsActiveLocksExists(long orderId)
        {
            throw new NotImplementedException();
        }

        public bool IsNonDeletedLocksExists(long orderId)
        {
            throw new NotImplementedException();
        }

        public bool IsInactiveLocksExists(long destinationOrganizationUnitId, TimePeriod period)
        {
            throw new NotImplementedException();
        }

        public int GetNonDeletedLocksCount(long orderId)
        {
            throw new NotImplementedException();
        }

        public int GetActiveLocksCount(long orderId)
        {
            throw new NotImplementedException();
        }

        public int GetInactiveLocksCount(long orderId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Limit> GetClosedLimits(long destinationOrganizationUnitId, TimePeriod period)
        {
            throw new NotImplementedException();
        }

        public int DeleteLocksForPeriod(long destinationOrganizationUnitId, TimePeriod period)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccountBalance(IEnumerable<long> accountIds)
        {
            UpdatedAccountIds.AddRange(accountIds);
        }

        public decimal? CalculateDebitForOrderPaymentSum(long orderId)
        {
            throw new NotImplementedException();
        }

        public void RecalculateLockValue(Lock lockEntity)
        {
            throw new NotImplementedException();
        }

        public Account FindAccount(long entityId)
        {
            throw new NotImplementedException();
        }

        public Account FindAccount(long branchOfficeOrganizationUnitId, long legalPersonId)
        {
            throw new NotImplementedException();
        }

        public bool IsCreateAccountDetailValid(long accountId, long userCode, bool checkContributionType)
        {
            throw new NotImplementedException();
        }

        public AccountDetail[] GetAccountDetailsForImportFrom1COperation(string branchOfficeOrganizationUnit1CCode, DateTime transactionPeriodStart, DateTime transactionPeriodEnd)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAccountsByLegalPerson(long legalPersonId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Account> GetAccountsByLegalPerson(string legalPersonSyncCode1C)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountFor1CExportDto> GetAccountsForExortTo1C(long organizationUnitId)
        {
            throw new NotImplementedException();
        }

        public Account CreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            throw new NotImplementedException();
        }

        public Account SecureCreateAccount(long legalPersonId, long branchOfficeOrganizationUnitId)
        {
            throw new NotImplementedException();
        }

        public int Create(OperationType operationType)
        {
            throw new NotImplementedException();
        }

        public int Create(AccountDetail accountDetail)
        {
            CreatedAccountDetails.Add(accountDetail);
            return 1;
        }

        public int Create(IEnumerable<AccountDetail> accountDetails)
        {
            if (accountDetails == null)
            {
                return 0;
            }

            var items = accountDetails as AccountDetail[] ?? accountDetails.ToArray();

            CreatedAccountDetails.AddRange(items);
            return items.Count();
        }

        public int Create(Lock @lock)
        {
            throw new NotImplementedException();
        }

        public int Update(Account account)
        {
            throw new NotImplementedException();
        }

        public int Update(IEnumerable<Account> accounts)
        {
            throw new NotImplementedException();
        }

        public int Update(OperationType operationType)
        {
            throw new NotImplementedException();
        }

        public int Update(AccountDetail accountDetail)
        {
            throw new NotImplementedException();
        }

        public int Update(Lock @lock)
        {
            throw new NotImplementedException();
        }

        public int Update(IEnumerable<Lock> locks)
        {
            throw new NotImplementedException();
        }

        public int Update(LockDetail lockDetail)
        {
            throw new NotImplementedException();
        }

        public int Update(IEnumerable<LockDetail> lockDetails)
        {
            throw new NotImplementedException();
        }

        public int Delete(Account account)
        {
            throw new NotImplementedException();
        }

        public int Delete(AccountDetail accountDetail)
        {
            DeletedAccountDetails.Add(accountDetail);
            return 1;
        }

        public int Delete(IEnumerable<AccountDetail> accountDetails)
        {
            if (accountDetails == null)
            {
                return 0;
            }

            var items = accountDetails as AccountDetail[] ?? accountDetails.ToArray();

            DeletedAccountDetails.AddRange(items);
            return items.Count();
        }

        public int Delete(LockDetail entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(Lock entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(OperationType entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OperationType> GetOperationTypes(string syncCode1C)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountInfoForImportFrom1C> GetAccountsForImportFrom1C(IEnumerable<string> branchOfficeSyncCodes, DateTime transactionPeriodStart, DateTime transactionPeriodEnd)
        {
            return AccountInfos.Where(x => branchOfficeSyncCodes.Contains(x.BranchOfficeSyncCode1C)).ToArray();
        }

        public IReadOnlyCollection<OperationType> GetOperationsInSyncWith1C()
        {
            return OperationsInSyncWith1C;
        }
    }
}