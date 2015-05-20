using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteAccountDetailOperationService : IDeleteGenericEntityService<AccountDetail>
    {
        private readonly IAccountRepository _accountRepository;

        public DeleteAccountDetailOperationService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var accountDetail = _accountRepository.GetAccountDetail(entityId);
                _accountRepository.Delete(accountDetail);
                _accountRepository.UpdateAccountBalance(new[] { accountDetail.AccountId });

                transaction.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var accountDetail = _accountRepository.GetAccountDetail(entityId);

            if (accountDetail == null)
                return new DeleteConfirmationInfo
                    {
                        IsDeleteAllowed = false,
                        DeleteDisallowedReason = BLResources.EntityNotFound
                    };

            return new DeleteConfirmationInfo
            {
                EntityCode = accountDetail.Comment,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}