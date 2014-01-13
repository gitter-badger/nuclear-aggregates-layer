using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateLimitService : IDeactivateGenericEntityService<Limit>
    {
        private readonly IAccountRepository _accountRepository;

        public DeactivateLimitService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _accountRepository as IDeactivateAggregateRepository<Limit>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }

            return null;
        }
    }
}