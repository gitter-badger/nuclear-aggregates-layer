using DoubleGis.Erm.BL.Aggregates.Clients;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.Operations.Generic.Modify.Old
{
    // 2+: BLFlex\Source\ApplicationServices\2Gis.Erm.BLFlex.Operations.Global\Russia\Generic\Modify\Old\EditClientHandler.cs
    public sealed class EditClientHandler : RequestHandler<EditRequest<Client>, EmptyResponse>, IRussiaAdapted
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;

        public EditClientHandler(
            IClientRepository clientRepository,
            IOperationScopeFactory operationScopeFactory,
            IUserContext userContext,
            ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
        {
            _clientRepository = clientRepository;
            _operationScopeFactory = operationScopeFactory;
            _userContext = userContext;
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
        }

        protected override EmptyResponse Handle(EditRequest<Client> request)
        {
            var client = request.Entity;
            EnsureAdvertisingAgensyManagementRights(client);

            using (var operationScope = _operationScopeFactory.CreateOrUpdateOperationFor(client))
            {
                var isNew = client.IsNew();

                _clientRepository.CreateOrUpdate(client);

                if (isNew)
                {
                    operationScope.Added<Client>(client.Id);
                }
                else
                {
                    operationScope.Updated<Client>(client.Id);
                }

                operationScope.Complete();
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
                var currentClientValue = _clientRepository.GetClient(client.Id);
                if (currentClientValue.IsAdvertisingAgency != client.IsAdvertisingAgency)
                {
                    throw new BusinessLogicException(BLResources.MustHaveFunctionalPrivilegeToChangeIsAdvertisingAgencyProperty);
                }
            }
        }
    }
}
