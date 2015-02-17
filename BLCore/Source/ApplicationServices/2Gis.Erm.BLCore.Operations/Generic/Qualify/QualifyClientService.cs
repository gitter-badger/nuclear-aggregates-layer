using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Qualify
{
    public class QualifyClientService : IQualifyGenericEntityService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IClientReadModel _readModel;
        private readonly ICommonLog _logger;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly IAssignAppointmentAggregateService _assignAppointmentAggregateService;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;

        public QualifyClientService(
            IUserContext userContext,
            IClientRepository clientRepository,
            ISecurityServiceUserIdentifier userIdentifierService,
            IOperationScopeFactory scopeFactory,
            IAppointmentReadModel appointmentReadModel,
            ILetterReadModel letterReadModel,
            IPhonecallReadModel phonecallReadModel,
            ITaskReadModel taskReadModel,
            IAssignAppointmentAggregateService assignAppointmentAggregateService,
            IAssignLetterAggregateService assignLetterAggregateService,
            IAssignPhonecallAggregateService assignPhonecallAggregateService,
            IAssignTaskAggregateService assignTaskAggregateService,
            IClientReadModel readModel,
            ICommonLog logger)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
            _userIdentifierService = userIdentifierService;
            _scopeFactory = scopeFactory;
            _readModel = readModel;
            _logger = logger;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentAggregateService = assignAppointmentAggregateService;
            _assignLetterAggregateService = assignLetterAggregateService;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        public QualifyResult Qualify(long entityId, long ownerCode, long? relatedEntityId)
        {
            var currentUser = _userContext.Identity;
            var reserveUser = _userIdentifierService.GetReserveUserIdentity();

            using (var operationScope = _scopeFactory.CreateSpecificFor<QualifyIdentity, Client>())
            {
                var qualifyAggregateRepository = _clientRepository as IQualifyAggregateRepository<Client>;
                qualifyAggregateRepository.Qualify(entityId, currentUser.Code, reserveUser.Code, ownerCode, DateTime.UtcNow);

                operationScope
                    .Updated<Client>(entityId)
                    .Complete();
            }

            AssignRelatedActivities(entityId, ownerCode);

            _logger.InfoFormat("[ERM] Клиент с id={0} взят из резерва, с назначением пользователю с id={1}", entityId, ownerCode);

            return new QualifyResult
                {
                    EntityId = entityId,
                    RelatedEntityId = relatedEntityId,
                    Message = CreateMessageWhenHasParentAdAgency(entityId)
                };
        }

        private string CreateMessageWhenHasParentAdAgency(long entityId)
        {
            var agencyNames = string.Join("; ", _readModel.GetMasterAdvertisingAgencies(entityId).Select(ad => ad.Name));

            if (string.IsNullOrEmpty(agencyNames))
            {
                return string.Empty;
            }

            var client = _readModel.GetClient(entityId);
            return string.Format(BLResources.ClientFromReserveHasParentAdAgency, client.Name, agencyNames);
        }

        private void AssignRelatedActivities(long clientId, long newOwnerCode)
        {
            foreach (var appointment in _appointmentReadModel.LookupOpenAppointmentsRegarding(EntityType.Instance.Client(), clientId))
            {
                _assignAppointmentAggregateService.Assign(appointment, newOwnerCode);
            }
            foreach (var letter in _letterReadModel.LookupOpenLettersRegarding(EntityType.Instance.Client(), clientId))
            {
                _assignLetterAggregateService.Assign(letter, newOwnerCode);
            }
            foreach (var phonecall in _phonecallReadModel.LookupOpenPhonecallsRegarding(EntityType.Instance.Client(), clientId))
            {
                _assignPhonecallAggregateService.Assign(phonecall, newOwnerCode);
            }
            foreach (var task in _taskReadModel.LookupOpenTasksRegarding(EntityType.Instance.Client(), clientId))
            {
                _assignTaskAggregateService.Assign(task, newOwnerCode);
            }
        }
    }
}