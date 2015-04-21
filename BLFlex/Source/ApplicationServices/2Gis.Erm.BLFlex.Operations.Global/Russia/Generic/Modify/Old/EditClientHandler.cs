using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.Old
{
    public sealed class EditClientHandler : RequestHandler<EditRequest<Client>, EmptyResponse>, IRussiaAdapted
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;
        private readonly ICreateClientAggregateService _createClientService;
        private readonly IUpdateAggregateRepository<Client> _updateClientService;
        private readonly IClientReadModel _clientReadModel;

        public EditClientHandler(IOperationScopeFactory operationScopeFactory,
                                 IUserContext userContext,
                                 ISecurityServiceFunctionalAccess securityServiceFunctionalAccess,
                                 ICreateClientAggregateService createClientService,
                                 IUpdateAggregateRepository<Client> updateClientService,
                                 IClientReadModel clientReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _userContext = userContext;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
            _createClientService = createClientService;
            _updateClientService = updateClientService;
            _clientReadModel = clientReadModel;
        }

        protected override EmptyResponse Handle(EditRequest<Client> request)
        {
            var client = request.Entity;
            EnsureAdvertisingAgensyManagementRights(client);

            // COMMENT {y.baranihin, 17.06.2014}: При отдельной реализации Create и Update получаем в дальнейшем возможность проще растащить этот код на отдельные операции
            if (client.IsNew())
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Client>())
                {
                    FirmAddress mainFirmAddress;
                    _createClientService.Create(client, out mainFirmAddress);
                    operationScope.Added<Client>(client.Id);

                    operationScope.Complete();
                }
            }
            else
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Client>())
                {
                    _updateClientService.Update(client);
                    operationScope.Updated<Client>(client.Id);

                    operationScope.Complete();
                }
            }

            return Response.Empty;
        }

        private void EnsureAdvertisingAgensyManagementRights(Client client)
        {
            var hasUserPrivilegeToManageAdvertisingAgensies =
                _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, _userContext.Identity.Code);
            if (hasUserPrivilegeToManageAdvertisingAgensies)
            {
                return;
            }

            if (client.IsNew())
            {
                if (client.IsAdvertisingAgency)
                {
                    throw new BusinessLogicException(BLResources.MustHaveFunctionalPrivilegeToChangeIsAdvertisingAgencyProperty);
                }
            }
            else
            {
                var currentClientValue = _clientReadModel.GetClient(client.Id);
                if (currentClientValue.IsAdvertisingAgency != client.IsAdvertisingAgency)
                {
                    throw new BusinessLogicException(BLResources.MustHaveFunctionalPrivilegeToChangeIsAdvertisingAgencyProperty);
                }
            }
        }
    }
}
