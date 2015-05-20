using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Deactivate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Deactivate
{
    public class DeactivateOrganizationUnitOperationService : IDeactivateGenericEntityService<OrganizationUnit>
    {
        private readonly IUserRepository _userRepository;

        public DeactivateOrganizationUnitOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DeactivateConfirmation Deactivate(long entityId, long ownerCode)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var users = _userRepository.GetUsersByOrganizationUnit(entityId);
                if(users.Any())
                {
                    throw new NotificationException(BLResources.CantDeactivateOrganizationUnitLinkedWithActiveUsers);
                }

                var deactivateAggregateRepository = _userRepository as IDeactivateAggregateRepository<OrganizationUnit>;
                deactivateAggregateRepository.Deactivate(entityId);

                transaction.Complete();
            }

            return null;
        }
    }
}