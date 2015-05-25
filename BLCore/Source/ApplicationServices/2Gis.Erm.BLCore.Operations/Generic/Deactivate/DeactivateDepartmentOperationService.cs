using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateDepartmentOperationService : IDeactivateGenericEntityService<Department>
    {
        private readonly IUserRepository _userRepository;

        public DeactivateDepartmentOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var deactivateAggregateRepository = _userRepository as IDeactivateAggregateRepository<Department>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }
            return null;
        }
    }
}