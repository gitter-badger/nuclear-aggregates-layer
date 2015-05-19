using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Activate
{
    public class ActivateDepartmentOperationService : IActivateGenericEntityService<Department>
    {
        private readonly IUserRepository _userRepository;

        public ActivateDepartmentOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public int Activate(long entityId)
        {
            int result = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var activateAggregateRepository = _userRepository as IActivateAggregateRepository<Department>;
                result = activateAggregateRepository.Activate(entityId);

                transaction.Complete();
            }
            return result;
        }
    }
}