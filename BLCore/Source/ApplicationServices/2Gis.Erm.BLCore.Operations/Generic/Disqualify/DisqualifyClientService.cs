using System;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Disqualify
{
    public class DisqualifyClientService : IDisqualifyGenericEntityService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICommonLog _logger;
        private readonly IClientReadModel _clientReadModel;
        private readonly IActivityReadService _activityReadService;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly IAssignAppointmentAggregateService _assignAppointmentAggregateService;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;

        public DisqualifyClientService(IUserContext userContext,
                                       IClientRepository clientRepository,
                                       ISecurityServiceUserIdentifier userIdentifierService,
                                       ISecurityServiceFunctionalAccess functionalAccessService,
                                       ICommonLog logger,
                                       ISecurityServiceEntityAccess securityServiceEntityAccess,
                                       IClientReadModel clientReadModel,
                                       IActivityReadService activityReadService,
                                       IAppointmentReadModel appointmentReadModel,
                                       ILetterReadModel letterReadModel,
                                       IPhonecallReadModel phonecallReadModel,
                                       ITaskReadModel taskReadModel,
                                       IAssignAppointmentAggregateService assignAppointmentAggregateService,
                                       IAssignLetterAggregateService assignLetterAggregateService,
                                       IAssignPhonecallAggregateService assignPhonecallAggregateService,
                                       IAssignTaskAggregateService assignTaskAggregateService)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
            _userIdentifierService = userIdentifierService;
            _functionalAccessService = functionalAccessService;
            _logger = logger;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _clientReadModel = clientReadModel;
            _activityReadService = activityReadService;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentAggregateService = assignAppointmentAggregateService;
            _assignLetterAggregateService = assignLetterAggregateService;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        public virtual DisqualifyResult Disqualify(long entityId, bool bypassValidation)
        {
            var client = _clientReadModel.GetClient(entityId);
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
                // Проверяем открытые связанные объекты:
                // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данным Клиентом и его фирмами, 
                // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данным Клиентом и его фирмами".
                var hasRelatedOpenedActivities = _activityReadService.CheckIfOpenActivityExistsRegarding(EntityName.Client, entityId);
                if (hasRelatedOpenedActivities)
                {
                    throw new NotificationException(BLResources.NeedToCloseAllActivities);
                }
                
                var reserveUser = _userIdentifierService.GetReserveUserIdentity();

                // изменения всех сущностей ERM выполняем в отдельной транзакции, чтобы все изменения сущностей среплицировались в MSCRM и транзакция была закрыта
                var disqualifyAggregateRepository = _clientRepository as IDisqualifyAggregateRepository<Client>;
                disqualifyAggregateRepository.Disqualify(entityId, _userContext.Identity.Code, reserveUser.Code, bypassValidation, DateTime.UtcNow);

                AssignRelatedActivities(client.Id, reserveUser.Code);

                _logger.InfoFormat("Клиент с id={0} возвращен в резерв", entityId);

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

        private void AssignRelatedActivities(long clientId, long newOwnerCode)
        {
            foreach (var appointment in _appointmentReadModel.LookupOpenAppointmentsRegarding(EntityName.Client, clientId))
            {
                _assignAppointmentAggregateService.Assign(appointment, newOwnerCode);
            }
            foreach (var letter in _letterReadModel.LookupOpenLettersRegarding(EntityName.Client, clientId))
            {
                _assignLetterAggregateService.Assign(letter, newOwnerCode);
            }
            foreach (var phonecall in _phonecallReadModel.LookupOpenPhonecallsRegarding(EntityName.Client, clientId))
            {
                _assignPhonecallAggregateService.Assign(phonecall, newOwnerCode);
            }
            foreach (var task in _taskReadModel.LookupOpenTasksRegarding(EntityName.Client, clientId))
            {
                _assignTaskAggregateService.Assign(task, newOwnerCode);
            }
        }
    }
}