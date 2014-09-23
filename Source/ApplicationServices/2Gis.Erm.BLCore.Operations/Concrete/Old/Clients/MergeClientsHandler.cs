using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public sealed class MergeClientsHandler : RequestHandler<MergeClientsRequest, EmptyResponse>
    {
        private readonly IClientRepository _clientRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;
        private readonly IAssignAppointmentAggregateService _assignAppointmentAggregateService;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;

        public MergeClientsHandler(IClientRepository clientRepository,
                                   IOperationScopeFactory scopeFactory,
                                   IAppointmentReadModel appointmentReadModel,
                                   ILetterReadModel letterReadModel,
                                   IPhonecallReadModel phonecallReadModel,
                                   ITaskReadModel taskReadModel,
                                   IAssignAppointmentAggregateService assignAppointmentAggregateService,
                                   IAssignLetterAggregateService assignLetterAggregateService,
                                   IAssignPhonecallAggregateService assignPhonecallAggregateService,
                                   IAssignTaskAggregateService assignTaskAggregateService
            )
        {
            _clientRepository = clientRepository;
            _scopeFactory = scopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentAggregateService = assignAppointmentAggregateService;
            _assignLetterAggregateService = assignLetterAggregateService;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
            _assignTaskAggregateService = assignTaskAggregateService;
        }

        protected override EmptyResponse Handle(MergeClientsRequest request)
        {
            if (request.AppendedClientId == request.Client.Id)
            {
                throw new NotificationException(BLResources.MergeClientsSameIdError);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<MergeIdentity, Client>())
            {
                _clientRepository.MergeErmClients(request.Client.Id, request.AppendedClientId, request.Client, request.AssignAllObjects);

                operationScope
                    .Updated<Client>(request.Client.Id, request.AppendedClientId)
                    .Complete();
            }

            if (request.AssignAllObjects)
            {
                // обновляем куратора у активностей, привязанных к удаляемому клиенту
                AssignRelatedActivities(request.AppendedClientId, request.Client.OwnerCode);
            }

            return Response.Empty;
        }

        private void AssignRelatedActivities(long clientId, long newOwnerCode)
        {
            foreach (var appointment in _appointmentReadModel.LookupAppointmentsRegarding(EntityName.Client, clientId))
            {
                _assignAppointmentAggregateService.Assign(appointment, newOwnerCode);
            }
            foreach (var letter in _letterReadModel.LookupLettersRegarding(EntityName.Client, clientId))
            {
                _assignLetterAggregateService.Assign(letter, newOwnerCode);
            }
            foreach (var phonecall in _phonecallReadModel.LookupPhonecallsRegarding(EntityName.Client, clientId))
            {
                _assignPhonecallAggregateService.Assign(phonecall, newOwnerCode);
            }
            foreach (var task in _taskReadModel.LookupTasksRegarding(EntityName.Client, clientId))
            {
                _assignTaskAggregateService.Assign(task, newOwnerCode);
            }
        }
    }
}