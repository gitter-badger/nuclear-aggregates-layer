using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public sealed class AccountRevertWithdrawFromAccountsAggregateService : IAccountRevertWithdrawFromAccountsAggregateService
    {
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<AccountDetail> _accountDetailRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;

        public AccountRevertWithdrawFromAccountsAggregateService(
            IRepository<Account> accountRepository, 
            IRepository<AccountDetail> accountDetailRepository,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger)
        {
            _accountRepository = accountRepository;
            _accountDetailRepository = accountDetailRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void RevertWithdraw(IEnumerable<RevertWithdrawFromAccountsDto> withdrawInfos)
        {
            _logger.InfoEx("Started reverting withdrawals process for accounts");

            using (var scope = _scopeFactory.CreateNonCoupled<RevertWithdrawFromAccountsIdentity>())
            {
                int processedAccounts = 0;
                int processedAccountDetails = 0;

                foreach (var info in withdrawInfos)
                {
                    _logger.DebugFormatEx("Deleting account details for withdrawal by account with id {0}", info.Account.Id);

                    info.Account.Balance = info.BalanceBeforeRevertWithdrawal;

                    foreach (var detail in info.DebitDetails)
                    {
                        info.Account.Balance += detail.Amount;
                        _accountDetailRepository.Delete(detail);
                        scope.Deleted<AccountDetail>(detail.Id);
                        ++processedAccountDetails;
                    }
                    
                    _accountRepository.Update(info.Account);
                    scope.Updated<Account>(info.Account.Id);
                    ++processedAccounts;
                }

                _logger.InfoFormatEx("During withdrawal reverting process {0} account details was deleted", processedAccountDetails);
                _logger.InfoFormatEx("Actualize accounts balances during withdrawal process. Accounts count: {0}", processedAccounts);

                _accountDetailRepository.Save();
                _accountRepository.Save();
                scope.Complete();
            }

            _logger.InfoEx("Finished reverting withdrawal process for accounts");
        }
    }
}