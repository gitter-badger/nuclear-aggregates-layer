using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Operations.Generic.Append
{
    public class AppendClientClientService : IAppendGenericEntityService<Client, Client>
    {
        private readonly ICreateClientLinkAggregateService _createAggregateService;
        private readonly IClientReadModel _clientReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly IUserContext _userContext;

        public AppendClientClientService(ICreateClientLinkAggregateService createClientLinkRepostiry,
                                         IClientReadModel readModel,
                                         IOperationScopeFactory scopeFactory,
                                         ISecurityServiceEntityAccess securityServiceEntityAccess,
                                         IUserContext userContext)
        {
            _createAggregateService = createClientLinkRepostiry;
            _clientReadModel = readModel;
            _scopeFactory = scopeFactory;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _userContext = userContext;
        }

        public void Append(AppendParams appendParams)
        {
            if (appendParams == null)
            {
                throw new ArgumentNullException("appendParams");
            }

            if (appendParams.ParentId == null || appendParams.AppendedId == null)
            {
                throw new ParentOrChildIdsNotSpecifiedException(Resources.Server.Properties.Resources.ParentOrChildIdsNotSpecified);
            }

            if (!appendParams.ParentType.Equals(EntityType.Instance.Client()) || !appendParams.AppendedType.Equals(EntityType.Instance.Client()))
            {
                throw new InvalidEntityTypesForLinkingException(Resources.Server.Properties.Resources.InvalidEntityTypesForLinking);
            }

            if (appendParams.AppendedId == appendParams.ParentId)
            {
                throw new SameIdsForEntitiesToLinkException(Resources.Server.Properties.Resources.SameIdsForEntitiesToLink);
            }

            using (var scope = _scopeFactory.CreateSpecificFor<AppendIdentity, Client, Client>())
            {
                var clientLink = new ClientLink { MasterClientId = appendParams.ParentId.Value, ChildClientId = appendParams.AppendedId.Value, IsDeleted = false };
                var childClient = _clientReadModel.GetClient(clientLink.ChildClientId);
                if (
                    !_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                                  EntityType.Instance.Client(),
                                                                  _userContext.Identity.Code,
                                                                  clientLink.ChildClientId,
                                                                  childClient.OwnerCode,
                                                                  null))
                {
                    throw new OperationAccessDeniedException(string.Format(BLResources.AccessDeniedToMakeClientLink, childClient.Name));
                }

                if (childClient.IsAdvertisingAgency)
                {
                    throw new AppendAdvertisingAgencyException(Resources.Server.Properties.Resources.AnAdvertisingAgencyCannotBeSpecifiedAsAChildClient);
                }

                if (_clientReadModel.IsClientLinksExists(clientLink.MasterClientId, clientLink.ChildClientId, clientLink.IsDeleted))
                {
                    throw new ClientLinkAlreadyExistsException(Resources.Server.Properties.Resources.ClientLinkAlreadyExists);
                }

                _createAggregateService.Create(clientLink, _clientReadModel.GetCurrentDenormalizationForClientLink(clientLink.MasterClientId, clientLink.ChildClientId));

                scope.Updated<Client>(clientLink.MasterClientId)
                     .Updated<Client>(clientLink.ChildClientId)
                     .Updated<ClientLink>(clientLink.Id);

                scope.Complete();
            }
        }
    }
}