using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Delete
{
    public class DeleteLegalPersonProfileService : IDeleteGenericEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonReadModel _readModel;
        private readonly IDeleteAggregateRepository<LegalPersonProfile> _deleteRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;

        public DeleteLegalPersonProfileService(ILegalPersonReadModel readModel,
                                               ISecurityServiceFunctionalAccess functionalAccessService,
                                               IUserContext userContext,
                                               IDeleteAggregateRepository<LegalPersonProfile> deleteRepository)
        {
            _readModel = readModel;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _deleteRepository = deleteRepository;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            try
            {
                    var legalPersonProfile = _readModel.GetLegalPersonProfile(entityId);

                    if (legalPersonProfile == null)
                    {
                        throw new NotificationException(BLResources.EntityNotFound);
                    }

                    var isDeleteAllowed = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.DeleteLegalPersonProfile,
                                                                                                 _userContext.Identity.Code);

                    if (!isDeleteAllowed)
                    {
                        throw new NotificationException(BLResources.AccessDenied);
                    }

                    if (legalPersonProfile.IsDeleted || !legalPersonProfile.IsActive)
                    {
                        throw new NotificationException(BLResources.LegalPersonProfileIsDeletedAlready);
                    }

                    if (legalPersonProfile.IsMainProfile)
                    {
                        throw new NotificationException(BLResources.CantDeleteMainLegalPersonProfile);
                    }

                _deleteRepository.Delete(entityId);

                    // FIXME {all, 28.10.2013}: По факту профиль не удаляется, а деактивируется
                // COMMENT {all, 25.04.2014}: такие требования
            }
            catch (SecurityException ex)
            {
                throw new NotificationException(ex.Message, ex);
            }

            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var legalPersonProfile = _readModel.GetLegalPersonProfile(entityId);

            if (legalPersonProfile == null)
            {
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };
            }

            var isDeleteAllowed = _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.DeleteLegalPersonProfile,
                                                                                         _userContext.Identity.Code);

            if (!isDeleteAllowed)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = legalPersonProfile.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityAccessDenied
                };
            }

            // уже удален
            if (legalPersonProfile.IsDeleted)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = legalPersonProfile.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.LegalPersonProfileIsDeletedAlready
                };
            }

            if (legalPersonProfile.IsMainProfile)
            {
                return new DeleteConfirmationInfo
                {
                    EntityCode = legalPersonProfile.Name,
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.CantDeleteMainLegalPersonProfile
                };
            }

            return new DeleteConfirmationInfo
            {
                EntityCode = legalPersonProfile.LegalPerson.ShortName,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}
