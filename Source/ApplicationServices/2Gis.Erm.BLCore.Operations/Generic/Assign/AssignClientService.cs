using System.Security;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignClientService : IAssignGenericEntityService<Client>
    {
        private readonly IPublicService _publicService;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICommonLog _logger;
        private readonly IClientReadModel _clientReadModel;

        public AssignClientService(
            IPublicService publicService,
            IClientRepository clientRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger,
            IClientReadModel clientReadModel)
        {
            _publicService = publicService;
            _clientRepository = clientRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _clientReadModel = clientReadModel;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Client>())
                {
                    _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Client> { Id = entityId });

                    var checkAggregateForDebtsRepository = _clientRepository as ICheckAggregateForDebtsRepository<Client>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, bypassValidation);

                    _clientRepository.AssignWithRelatedEntities(entityId, ownerCode, isPartialAssign);

                    operationScope
                        .Updated<Client>(entityId)
                        .Complete();
                }

                _logger.InfoFormatEx("[ERM] Куратором клиента с id={0} назначен пользователь {1}, isPartialAssign = {2}", entityId, ownerCode, isPartialAssign);

                _publicService.Handle(new AssignClientRelatedEntitiesRequest
                    {
                        ClientId = entityId,
                        OwnerCode = ownerCode,
                        IsPartial = isPartialAssign
                    });

                _logger.InfoFormatEx("[CRM] Куратором клиента с id={0} назначен пользователь {1}, isPartialAssign = {2}", entityId, ownerCode, isPartialAssign);

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

                return new AssignResult 
                    {
                        EntityId = entityId,
                        OwnerCode = ownerCode,
                        CanProceed = true,
                        Message = ex.Message
                    };
            }
            catch (SecurityAccessDeniedException ex)
            {
                var clientName = _clientReadModel.GetClientName(entityId);
                if (clientName != null)
                {
                    throw new SecurityException(string.Format(BLResources.AccessToClientIsDenied, clientName), ex.InnerException);
                }

                throw;
            }
        }
    }
}
