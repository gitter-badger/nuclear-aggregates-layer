using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountWithdrawFromAccountsAggregateService : IAccountWithdrawFromAccountsAggregateService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<AccountDetail> _accountDetailRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountWithdrawFromAccountsAggregateService(
            IRepository<Account> accountRepository, 
            IRepository<AccountDetail> accountDetailRepository, 
            IIdentityProvider identityProvider, 
            IOperationScopeFactory scopeFactory, 
            ICommonLog logger)
        {
            _accountRepository = accountRepository;
            _accountDetailRepository = accountDetailRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public IReadOnlyDictionary<long, long> Withdraw(
            IEnumerable<WithdrawFromAccountsDto> withdrawInfos, 
            long debitForOrderPaymentOperationTypeId,
            DateTime releaseDate)
        {
            var accountDetailsMap = new Dictionary<long, long>();

            _logger.Info("Started withdrawal process for accounts");

            using (var scope = _scopeFactory.CreateNonCoupled<WithdrawFromAccountsIdentity>())
            {
                var accounts = new Dictionary<long, Account>();
                foreach (var info in withdrawInfos)
                {
                    var nowDate = DateTime.UtcNow;
                    _logger.DebugFormat("Creating account detail for withdrawal by account with id {0}", info.Account.Id);
                    var accountDetail = new AccountDetail
                    {
                        AccountId = info.Account.Id,
                        Amount = info.LockBalance,
                        TransactionDate = nowDate,
                        CreatedOn = nowDate,
                        OwnerCode = info.Account.OwnerCode,
                        OperationTypeId = debitForOrderPaymentOperationTypeId,
                        IsActive = true,
                        Description = string.Format(BLResources.ChargeAsPaymentForOrder, info.OrderNumber, releaseDate.ToShortDateString())
                    };

                    _identityProvider.SetFor(accountDetail);
                    _accountDetailRepository.Add(accountDetail);
                    scope.Added<AccountDetail>(accountDetail.Id);
                    accountDetailsMap.Add(info.LockId, accountDetail.Id);

                    Account targetAccount;
                    if (!accounts.TryGetValue(info.Account.Id, out targetAccount))
                    {
                        targetAccount = info.Account;
                        targetAccount.Balance = info.BalanceBeforeWithdrawal;
                        accounts.Add(info.Account.Id, targetAccount);
                    }

                    targetAccount.Balance = targetAccount.Balance - accountDetail.Amount;
                }

                _logger.InfoFormat("During withdrawal process {0} account details was created", accountDetailsMap.Count);
                _logger.InfoFormat("Actualize accounts balances during withdrawal process. Accounts count: {0}", accounts.Count);

                foreach (var account in accounts.Values)
                {
                    _accountRepository.Update(account);
                    scope.Updated<Account>(account.Id);
                }

                _accountRepository.Save();
                _accountDetailRepository.Save();
                scope.Complete();
            }

            _logger.Info("Finished withdrawal process for accounts");

            return accountDetailsMap;
        }
    }
}