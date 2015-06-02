using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteUserOrganizationUnitOperationService : IDeleteGenericEntityService<UserOrganizationUnit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserOrganizationUnitOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var deleteRepository = (IDeleteAggregateRepository<UserOrganizationUnit>) _userRepository;
            deleteRepository.Delete(entityId);
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