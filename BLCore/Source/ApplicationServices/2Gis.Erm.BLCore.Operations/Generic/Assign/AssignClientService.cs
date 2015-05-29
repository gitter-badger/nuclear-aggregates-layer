using System.Linq;
using System.Security;
using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Assign
{
    public class AssignClientService : IAssignGenericEntityService<Client>
    {
        private readonly IPublicService _publicService;
        private readonly IClientService _clientRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IUserContext _userContext;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;
        private readonly IClientReadModel _clientReadModel;
        private readonly IAppointmentReadModel _appointmentReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly IAssignAppointmentAggregateService _assignAppointmentAggregateService;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;

        public AssignClientService(
            IPublicService publicService,
            IClientService clientRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext,
            IOperationScopeFactory scopeFactory,
            ITracer tracer,
            IClientReadModel clientReadModel,
            IAppointmentReadModel appointmentReadModel,
            IActionLogger actionLogger,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,
            ITaskReadModel taskReadModel,
            IAssignAppointmentAggregateService assignAppointmentAggregateService,
            IAssignLetterAggregateService assignLetterAggregateService,
            IAssignPhonecallAggregateService assignPhonecallAggregateService,
            IAssignTaskAggregateService assignTaskAggregateService)
        {
            _publicService = publicService;
            _clientRepository = clientRepository;
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
            _clientReadModel = clientReadModel;
            _appointmentReadModel = appointmentReadModel;
            _actionLogger = actionLogger;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentAggregateService = assignAppointmentAggregateService;
            _assignLetterAggregateService = assignLetterAggregateService;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        public virtual AssignResult Assign(long entityId, long ownerCode, bool bypassValidation, bool isPartialAssign)
        {
            try
            {
                // TODO {s.pomadin, 19.09.2014}: do we need to wrap into the transaction scope?
                var prevOwnerCode = _clientReadModel.GetClient(entityId).OwnerCode;

                using (var operationScope = _scopeFactory.CreateSpecificFor<AssignIdentity, Client>())
                {
                    _publicService.Handle(new ValidateOwnerIsNotReserveRequest<Client> { Id = entityId });

                    var checkAggregateForDebtsRepository = _clientRepository as ICheckAggregateForDebtsRepository<Client>;
                    checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, bypassValidation);

                    _clientRepository.AssignWithRelatedEntities(entityId, ownerCode, isPartialAssign);

                    operationScope.Updated<Client>(entityId)
                                  .Complete();
                }

                AssignRelatedActivities(entityId, prevOwnerCode, ownerCode, isPartialAssign);

                _tracer.InfoFormat("[ERM] Куратором клиента с id={0} назначен пользователь {1}, isPartialAssign = {2}", entityId, ownerCode, isPartialAssign);

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

        private void AssignRelatedActivities(long clientId, long prevOwnerCode, long newOwnerCode, bool isPartialAssign)
        {
            foreach (var appointment in _appointmentReadModel.LookupOpenAppointmentsRegarding(EntityType.Instance.Client(), clientId).Where(x => !isPartialAssign || x.OwnerCode == prevOwnerCode))
            {
                var originalOwner = appointment.OwnerCode;
                _assignAppointmentAggregateService.Assign(appointment, newOwnerCode);
                _actionLogger.LogChanges(appointment, x => x.OwnerCode, originalOwner, appointment.OwnerCode);
            }
            foreach (var letter in _letterReadModel.LookupOpenLettersRegarding(EntityType.Instance.Client(), clientId).Where(x => !isPartialAssign || x.OwnerCode == prevOwnerCode))
            {
                var originalOwner = letter.OwnerCode;
                _assignLetterAggregateService.Assign(letter, newOwnerCode);
                _actionLogger.LogChanges(letter, x => x.OwnerCode, originalOwner, letter.OwnerCode);
            }
            foreach (var phonecall in _phonecallReadModel.LookupOpenPhonecallsRegarding(EntityType.Instance.Client(), clientId).Where(x => !isPartialAssign || x.OwnerCode == prevOwnerCode))
            {
                var originalOwner = phonecall.OwnerCode;
                _assignPhonecallAggregateService.Assign(phonecall, newOwnerCode);
                _actionLogger.LogChanges(phonecall, x => x.OwnerCode, originalOwner, phonecall.OwnerCode);
            }
            foreach (var task in _taskReadModel.LookupOpenTasksRegarding(EntityType.Instance.Client(), clientId).Where(x => !isPartialAssign || x.OwnerCode == prevOwnerCode))
            {
                var originalOwner = task.OwnerCode;
                _assignTaskAggregateService.Assign(task, newOwnerCode);
                _actionLogger.LogChanges(task, x => x.OwnerCode, originalOwner, task.OwnerCode);
            }
        }
    }
}
