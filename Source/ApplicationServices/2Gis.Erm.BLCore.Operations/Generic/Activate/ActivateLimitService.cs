using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateLimitService : IActivateGenericEntityService<Limit>
    {
        private readonly IAccountRepository _accountRepository;

        public ActivateLimitService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public int Activate(long entityId)
        {
            int result = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = _accountRepository as IActivateAggregateRepository<Limit>;
                result = activateAggregateRepository.Activate(entityId);

                transaction.Complete();
            }
            return result;
        }
    }
}
