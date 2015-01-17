using System;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.API.Aggregates.Roles.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.Operation;
using DoubleGis.Erm.BLCore.API.Aggregates.Users.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using Microsoft.Crm.SdkTypeProxy;

using EntityName = DoubleGis.Erm.Platform.Model.Entities.EntityName;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Append
{
    public class AppendUserRoleOperationService : IAppendGenericEntityService<Role, User> 
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IUserReadModel _userReadModel;
        private readonly IRoleReadModel _roleReadModel;
        private readonly IUserAppendRoleAggregateService _userAppendRoleAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public AppendUserRoleOperationService(
            IMsCrmSettings msCrmSettings,
            IUserReadModel userReadModel,
            IRoleReadModel roleReadModel,
            IUserAppendRoleAggregateService userAppendRoleAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _msCrmSettings = msCrmSettings;
            _userReadModel = userReadModel;
            _roleReadModel = roleReadModel;
            _userAppendRoleAggregateService = userAppendRoleAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ArgumentException(BLResources.UserIdOrRoleIdIsNotSpecified);
            }

            if (appendParams.ParentType != EntityName.User || appendParams.AppendedType != EntityName.Role)
            {
                throw new ArgumentException(BLResources.EntityNamesShouldBeUserAndRole);
            }

            long userId = appendParams.ParentId.Value;
            long roleId = appendParams.AppendedId.Value;

            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, User, Role>())
            {
                var userAndRolesRelationsInfo = _userReadModel.GetUserWithRoleRelations(userId);
                if (userAndRolesRelationsInfo.RolesRelations.Any(relation => relation.RoleId == roleId))
                {
                    throw new BusinessLogicException(BLResources.RoleAlreadyAdded);
                }

                var targetRole = _roleReadModel.GetRole(roleId);
                if (targetRole == null)
                {
                    throw new EntityNotFoundException(typeof(Role));
                }

                _userAppendRoleAggregateService.AppendRole(userAndRolesRelationsInfo.User, roleId);

                if (!userAndRolesRelationsInfo.User.IsServiceUser)
                {
                    AppendRoleMsCRM(userAndRolesRelationsInfo.User.Account, targetRole.Name);
                }

                scope.Updated<User>(userId)
                     .Updated<Role>(roleId)
                     .Complete();
            }
        }

        private void AppendRoleMsCRM(string userAccount, string roleName)
        {
            if (!_msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Sdk))
            {
                return;
            }

            try
            {
                var crmDataContext = _msCrmSettings.CreateDataContext();
                var crmUserInfo = crmDataContext.GetSystemUserByDomainName(userAccount, true);

                Guid crmRoleId;
                if (!crmDataContext.TryGetCrmRoleId(crmUserInfo.BusinessUnitId, roleName, out crmRoleId))
                {
                    throw new BusinessLogicException(BLResources.RoleNotExistInCRM);
                }

                crmDataContext.UsingService(x => x.Execute(new AssignUserRolesRoleRequest { UserId = crmUserInfo.UserId, RoleIds = new[] { crmRoleId } }));
            }
            catch (WebException ex)
            {
                throw new BusinessLogicException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }
        } 
    }
}
