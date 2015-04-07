using System;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Resources.Server;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.Platform.DAL
{
    public class SecurityHelper
    {
        private readonly ISecurityServiceEntityAccessInternal _entityAccessService;
        private readonly IUserContext _userContext;

        public SecurityHelper(IUserContext userContext, ISecurityServiceEntityAccessInternal entityAccessService)
        {
            if (userContext == null)
            {
                throw new ArgumentNullException("userContext");
            }

            if (entityAccessService == null)
            {
                throw new ArgumentNullException("entityAccessService");
            }

            _userContext = userContext;
            _entityAccessService = entityAccessService;
        }

        public void CheckRequest<TEntity>(EntityAccessTypes operationType, TEntity entity) where TEntity : class, ICuratedEntity, IEntityKey
        {
            var securityControlAspect = _userContext.Identity as IUserIdentitySecurityControl;
            if (securityControlAspect != null && securityControlAspect.SkipEntityAccessCheck)
            {
                return;
            }

            var restrictedOperationType = _entityAccessService.RestrictEntityAccess(
                typeof(TEntity).AsEntityName(), 
                                                                                    operationType,
                                                                                    _userContext.Identity.Code,
                                                                                    entity.Id,
                                                                                    entity.OwnerCode,
                                                                                    entity.OldOwnerCode);

            if (restrictedOperationType != operationType)
            {
                throw NewSecurityAccessDeniedException<TEntity>(operationType);
            }
        }

        public void IsEntityAccessTypeGranted<TEntity>(EntityAccessTypes operationType) where TEntity : class, ICuratedEntity, IEntityKey
        {
            var entityAccess = _entityAccessService.GetCommonEntityAccessForMetadata(typeof(TEntity).AsEntityName(), _userContext.Identity.Code);
            if ((entityAccess & operationType) != operationType)
            {
                throw NewSecurityAccessDeniedException<TEntity>(operationType);
            }
        }

        private static string CreateErrorMessage(string userAccount, string accessType, string entityName)
        {
            return string.Format(ResPlatform.AccessDeniedUserActionEntityTemplate,
                                 Environment.NewLine,
                                 userAccount,
                                 accessType,
                                 entityName);
        }

        private SecurityAccessDeniedException NewSecurityAccessDeniedException<TEntity>(EntityAccessTypes operationType)
           where TEntity : class, ICuratedEntity, IEntityKey
        {
            return new SecurityAccessDeniedException(CreateErrorMessage(_userContext.Identity.Account, operationType.ToString(), typeof(TEntity).Name));
        }
    }
}
