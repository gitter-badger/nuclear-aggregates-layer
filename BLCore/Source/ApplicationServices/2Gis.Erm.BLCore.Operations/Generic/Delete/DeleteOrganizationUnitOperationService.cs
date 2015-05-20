using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteOrganizationUnitOperationService : IDeleteGenericEntityService<OrganizationUnit>
    {
        private readonly IUserRepository _userRepository;

        public DeleteOrganizationUnitOperationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var organizationUnitInfo = _userRepository.GetOrganizationUnitDetails(entityId);
            if (organizationUnitInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }
            if (organizationUnitInfo.HasLinkedUsers)
            {
                throw new ArgumentException(BLResources.OrganizationUnitHasUsers);
            }

            _userRepository.Delete(organizationUnitInfo.Unit);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var organizationUnitInfo = _userRepository.GetOrganizationUnitDetails(entityId); 

            if (organizationUnitInfo == null)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };

            if (organizationUnitInfo.HasLinkedUsers)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.OrganizationUnitHasUsers
                };

            return new DeleteConfirmationInfo
            {
                EntityCode = organizationUnitInfo.Unit.Name,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}