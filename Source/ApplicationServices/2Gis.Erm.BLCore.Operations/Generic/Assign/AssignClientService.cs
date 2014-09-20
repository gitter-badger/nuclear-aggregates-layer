using System.Security;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
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
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
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
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly IAssignGenericEntityService<Appointment> _assignAppointmentEntityService;
        private readonly IAssignGenericEntityService<Letter> _assignLetterEntityService;
        private readonly IAssignGenericEntityService<Phonecall> _assignPhonecallEntityService;
        private readonly IAssignGenericEntityService<Task> _assignTaskEntityService;

        public AssignClientService(
            IPublicService publicService,
            IClientRepository clientRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ICommonLog logger,
            IClientReadModel clientReadModel,
            IAppointmentReadModel appointmentReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,
            ITaskReadModel taskReadModel,
            IAssignGenericEntityService<Appointment> assignAppointmentEntityService,
            IAssignGenericEntityService<Letter> assignLetterEntityService,
            IAssignGenericEntityService<Phonecall> assignPhonecallEntityService,
            IAssignGenericEntityService<Task> assignTaskEntityService)
        {
            _publicService = publicService;
            _clientRepository = clientRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _clientReadModel = clientReadModel;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentEntityService = assignAppointmentEntityService;
            _assignLetterEntityService = assignLetterEntityService;
            _assignPhonecallEntityService = assignPhonecallEntityService;
            _assignTaskEntityService = assignTaskEntityService;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                // TODO {all, 19.09.2014}: do we need into wrap in the transaction scope?

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
                foreach (var activity in _appointmentReadModel.LookupRelatedActivities(EntityName.Client, entityId))
                {
                    _assignAppointmentEntityService.Assign(activity.Id, ownerCode, bypassValidation, isPartialAssign);
                }
                foreach (var activity in _letterReadModel.LookupRelatedActivities(EntityName.Client, entityId))
                {
                    _assignLetterEntityService.Assign(activity.Id, ownerCode, bypassValidation, isPartialAssign);
                }
                foreach (var activity in _phonecallReadModel.LookupRelatedActivities(EntityName.Client, entityId))
                {
                    _assignPhonecallEntityService.Assign(activity.Id, ownerCode, bypassValidation, isPartialAssign);
                }
                foreach (var activity in _taskReadModel.LookupRelatedActivities(EntityName.Client, entityId))
                {
                    _assignTaskEntityService.Assign(activity.Id, ownerCode, bypassValidation, isPartialAssign);
                }
//                _publicService.Handle(new AssignClientRelatedEntitiesRequest
//                    {
//                        ClientId = entityId,
//                        OwnerCode = ownerCode,
//                        IsPartial = isPartialAssign
//                    });
//
//                _logger.InfoFormatEx("[CRM] Куратором клиента с id={0} назначен пользователь {1}, isPartialAssign = {2}", entityId, ownerCode, isPartialAssign);

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
