﻿using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Delete
{
    public class DeleteLegalPersonProfileService : IDeleteGenericEntityService<LegalPersonProfile>
    {
        private readonly ILegalPersonReadModel _readModel;
        private readonly IDeleteLegalPersonProfileAggregateService _deleteLegalPersonProfileAggregateService;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        public DeleteLegalPersonProfileService(ILegalPersonReadModel readModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
                                               IDeleteLegalPersonProfileAggregateService deleteLegalPersonProfileAggregateService)
        {
            _readModel = readModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _deleteLegalPersonProfileAggregateService = deleteLegalPersonProfileAggregateService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            try
            {
                var legalPersonProfile = _readModel.GetLegalPersonProfile(entityId);

                if (legalPersonProfile == null)
                {
                    throw new EntityNotFoundException(typeof(LegalPersonProfile), entityId);
                }

                var isDeleteAllowed = _entityAccessService.HasEntityAccess(EntityAccessTypes.Delete,
                                                                           EntityName.LegalPersonProfile,
                                                                           _userContext.Identity.Code,
                                                                           legalPersonProfile.Id,
                                                                           legalPersonProfile.OwnerCode,
                                                                           null);

                if (!isDeleteAllowed)
                {
                    throw new OperationAccessDeniedException(DeleteIdentity.Instance);
                }

                if (legalPersonProfile.IsDeleted || !legalPersonProfile.IsActive)
                {
                    throw new NotificationException(BLResources.LegalPersonProfileIsDeletedAlready);
                }

                if (legalPersonProfile.IsMainProfile)
                {
                    throw new NotificationException(BLResources.CantDeleteMainLegalPersonProfile);
                }

                _deleteLegalPersonProfileAggregateService.Delete(legalPersonProfile);
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

            var isDeleteAllowed = _entityAccessService.HasEntityAccess(EntityAccessTypes.Delete,
                                                                       EntityName.LegalPersonProfile,
                                                                       _userContext.Identity.Code,
                                                                       legalPersonProfile.Id,
                                                                       legalPersonProfile.OwnerCode,
                                                                       null);

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
