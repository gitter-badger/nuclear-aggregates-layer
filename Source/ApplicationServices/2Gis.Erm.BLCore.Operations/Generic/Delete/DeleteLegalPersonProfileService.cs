using System.Security;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteLegalPersonProfileService : IDeleteGenericEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteLegalPersonProfileService(
            ILegalPersonRepository legalPersonRepository,
            ISecurityServiceFunctionalAccess functionalAccessService, 
            IUserContext userContext, IOperationScopeFactory scopeFactory)
        {
            _legalPersonRepository = legalPersonRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
                {
                    var findResult = _legalPersonRepository.FindLegalPersonProfile(entityId);

                    if (findResult == null)
                    {
                        throw new NotificationException(BLResources.EntityNotFound);
                    }

                    var isDeleteAllowed =
                        _functionalAccessService.HasFunctionalPrivilegeGranted(
                            FunctionalPrivilegeName.DeleteLegalPersonProfile, _userContext.Identity.Code);

                    if (!isDeleteAllowed)
                    {
                        throw new NotificationException(BLResources.AccessDenied);
                    }

                    if (findResult.IsDeleted||!findResult.IsActive)
                    {
                        throw new NotificationException(BLResources.LegalPersonProfileIsDeletedAlready);
                    }
                    if (findResult.IsMainProfile)
                    {
                        throw new NotificationException(BLResources.CantDeleteMainLegalPersonProfile);
                    }

                    _legalPersonRepository.Delete(findResult);

                    // FIXME {all, 28.10.2013}: По факту профиль не удаляется, а деактивируется
                    scope.Updated<LegalPersonProfile>(entityId).Complete();
                }
            }
            catch (SecurityException ex)
            {
                throw new NotificationException(ex.Message, ex);
            }
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var findResult = _legalPersonRepository.FindLegalPersonProfile(entityId);

            if (findResult == null)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };
            }

            var isDeleteAllowed =
                _functionalAccessService.HasFunctionalPrivilegeGranted(
                    FunctionalPrivilegeName.DeleteLegalPersonProfile, _userContext.Identity.Code);

            if (!isDeleteAllowed)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = findResult.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityAccessDenied
                };
            }

            // уже удален
            if (findResult.IsDeleted)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = findResult.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.LegalPersonProfileIsDeletedAlready
                };
            }
            if (findResult.IsMainProfile)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = findResult.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.CantDeleteMainLegalPersonProfile
                };
            }
            return new DeleteConfirmationInfo
            {
                EntityCode = findResult.LegalPerson.ShortName,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}
