using System;
using System.Security;

using DoubleGis.Erm.BLCore.Aggregates;
using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    public class DisqualifyClientService : IDisqualifyGenericEntityService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IPublicService _publicService;
        private readonly ICommonLog _logger;

        public DisqualifyClientService(IUserContext userContext, 
            IClientRepository clientRepository, 
            ISecurityServiceUserIdentifier userIdentifierService, 
            ISecurityServiceFunctionalAccess functionalAccessService, 
            IOperationScopeFactory scopeFactory,
            IPublicService publicService, 
            ICommonLog logger, 
            ISecurityServiceEntityAccess securityServiceEntityAccess)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _publicService = publicService;
            _logger = logger;
            _securityServiceEntityAccess = securityServiceEntityAccess;
        }

        public virtual DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            var client = _clientRepository.GetClient(entityId);
            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Update,
                                                              EntityName.Client,
                                                              _userContext.Identity.Code,
                                                              client.Id,
                                                              client.OwnerCode,
                                                              null))
            {
                throw new NotificationException(string.Format(BLResources.ClientDisqualificationIsDeniedForTheUser, client.Name));
            }

            try
            {
                // проверки активностей клиента в MSCRM (фактически выполняет только чтение), 
                // должен выполняться до любых изменений реплицируемых в MSCRM в рамках данной транзакции, чтобы избежать блокировки 
                _publicService.Handle(new CheckClientActivitiesRequest { ClientId = entityId });

                var reserveUser = _userIdentifierService.GetReserveUserIdentity();
                _publicService.Handle(new AssignClientRelatedEntitiesRequest { OwnerCode = reserveUser.Code, ClientId = entityId });

                // изменения всех сущностей ERM выполняем в отдельной транзакции, чтобы все изменения сущностей среплицировались в MSCRM и транзакция была закрыта
                var disqualifyAggregateRepository = _clientRepository as IDisqualifyAggregateRepository<Client>;
                disqualifyAggregateRepository.Disqualify(entityId, _userContext.Identity.Code, reserveUser.Code, bypassValidation, DateTime.UtcNow);


                _logger.InfoFormatEx("Клиент с id={0} возвращен в резерв", entityId);
                return null;
            }
            catch (ProcessAccountsWithDebtsException ex)
            {
                var hasProcessAccountsWithDebtsPermissionGranted =
                    _functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, _userContext.Identity.Code);
                if (!hasProcessAccountsWithDebtsPermissionGranted)
                {
                    throw new SecurityException(string.Format("{0}. {1}", BLResources.OperationNotAllowed, ex.Message), ex);
                }

                return new DisqualifyResult
                    {
                        EntityId = entityId,
                        CanProceed = true,
                        Message = ex.Message
                    };
            }
        }
    }
}