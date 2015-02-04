using System;
using System.Collections.Generic;
using System.Net;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.AccessSharing;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditAccessSharingHandler : RequestHandler<EditAccessSharingRequest, EmptyResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ISecurityServiceSharings _securityServiceSharings;
        private readonly IUserContext _userContext;

        public EditAccessSharingHandler(IMsCrmSettings msCrmSettings,  ISecurityServiceSharings securityServiceSharings, IUserContext userContext)
        {
            _msCrmSettings = msCrmSettings;
            _securityServiceSharings = securityServiceSharings;
            _userContext = userContext;
        }

        protected override EmptyResponse Handle(EditAccessSharingRequest request)
        {
            _securityServiceSharings.UpdateAccessSharings(request.EntityType, 
                request.EntityId, 
                request.EntityOwnerId, 
                request.AccessSharings, 
                _userContext.Identity.Code);

            if (_msCrmSettings.IntegrationMode.HasFlag(MsCrmIntegrationMode.Sdk))
            {
                var oldSharings = _securityServiceSharings.GetAccessSharingsForEntity(request.EntityType, request.EntityId);
                ReplicateAccessSharings(request, oldSharings, request.AccessSharings);
            }

            return Response.Empty;
        }

        private void ReplicateAccessSharings(EditAccessSharingRequest request, IEnumerable<SharingDescriptor> oldSharings, IEnumerable<SharingDescriptor> newSharings)
        {
            try
            {
                var crmDataContext = _msCrmSettings.CreateDataContext();

                var targetOwnedDynamic = new TargetOwnedDynamic
                {
                    EntityName = MapToCrmEntityType[request.EntityType.ToString()],
                    EntityId = request.EntityReplicationCode,
                };

                // remove old sharings
                foreach (var oldSharing in oldSharings)
                {
                    var userInfo = crmDataContext.GetSystemUserByDomainName(oldSharing.UserInfo.Account, true);
                    var revokee = new SecurityPrincipal { PrincipalId = userInfo.UserId, Type = SecurityPrincipalType.User };

                    crmDataContext.UsingService(service => service.Execute(new RevokeAccessRequest { Revokee = revokee, Target = targetOwnedDynamic }));
                }

                // add new sharings
                foreach (var newSharing in newSharings)
                {
                    var userInfo = crmDataContext.GetSystemUserByDomainName(newSharing.UserInfo.Account, true);
                    var grantee = new SecurityPrincipal { PrincipalId = userInfo.UserId, Type = SecurityPrincipalType.User };
                    var accessRights = MapToCrmAccessRights(newSharing.AccessTypes);

                    crmDataContext.UsingService(service => service.Execute(new GrantAccessRequest { PrincipalAccess = new PrincipalAccess { Principal = grantee, AccessMask = accessRights }, Target = targetOwnedDynamic }));
                }
            }
            catch (WebException ex)
            {
                throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }
        }

        #region access rights mapping between ERM and CRM

        private static readonly Dictionary<EntityAccessTypes, AccessRights> MapToCrmAccessRight = new Dictionary<EntityAccessTypes, AccessRights>
        {
            { EntityAccessTypes.Assign, AccessRights.AssignAccess },
            { EntityAccessTypes.Create, AccessRights.CreateAccess },
            { EntityAccessTypes.Delete, AccessRights.DeleteAccess },
            { EntityAccessTypes.Read, AccessRights.ReadAccess },
            { EntityAccessTypes.Share, AccessRights.ShareAccess },
            { EntityAccessTypes.Update, AccessRights.WriteAccess }
        };

        private static AccessRights MapToCrmAccessRights(EntityAccessTypes accessRights)
        {
            var mappedAccessRights = new AccessRights();
            foreach (EntityAccessTypes accessRight in Enum.GetValues(typeof(EntityAccessTypes)))
            {
                if (accessRights.HasFlag(accessRight))
                {
                    mappedAccessRights |= MapToCrmAccessRight[accessRight];
                }
            }

            return mappedAccessRights;
        }

        #endregion

        #region entity type name mapping between ERM and CRM

        private static readonly Dictionary<string, string> MapToCrmEntityType = new Dictionary<string, string>
        {
            // todo: заполнить все на свете сущности
            { "Client", "account" },
        };

        #endregion
    }
}