using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Roles;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteRoleOperationService : IDeleteGenericEntityService<Role>
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleOperationService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var hasUsers = _roleRepository.HasUsers(entityId);
            if (hasUsers)
            {
                throw new NotificationException(BLResources.RoleIsLinkedWithActiveUsers);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                _roleRepository.Delete(entityId);
                transaction.Complete();
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            return new DeleteConfirmationInfo
            {
                EntityCode = string.Empty,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}