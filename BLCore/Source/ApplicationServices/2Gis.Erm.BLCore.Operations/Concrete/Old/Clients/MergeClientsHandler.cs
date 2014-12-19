using System.Collections.Generic;
using System.Linq;

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
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
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
        private readonly IUpdateAppointmentAggregateService _updateAppointmentAggregateService;
        private readonly IAssignLetterAggregateService _assignLetterAggregateService;
        private readonly IUpdateLetterAggregateService _updateLetterAggregateService;
        private readonly IAssignPhonecallAggregateService _assignPhonecallAggregateService;
        private readonly IUpdatePhonecallAggregateService _updatePhonecallAggregateService;
        private readonly IAssignTaskAggregateService _assignTaskAggregateService;
        private readonly IUpdateTaskAggregateService _updateTaskAggregateService;

        public MergeClientsHandler(IClientRepository clientRepository,
                                   IOperationScopeFactory scopeFactory,
                                   IAppointmentReadModel appointmentReadModel,
                                   ILetterReadModel letterReadModel,
                                   IPhonecallReadModel phonecallReadModel,
                                   ITaskReadModel taskReadModel,
                                   IAssignAppointmentAggregateService assignAppointmentAggregateService,
                                   IUpdateAppointmentAggregateService updateAppointmentAggregateService,
                                   IAssignLetterAggregateService assignLetterAggregateService,
                                   IUpdateLetterAggregateService updateLetterAggregateService,
                                   IAssignPhonecallAggregateService assignPhonecallAggregateService,
                                   IUpdatePhonecallAggregateService updatePhonecallAggregateService,
                                   IAssignTaskAggregateService assignTaskAggregateService,
                                   IUpdateTaskAggregateService updateTaskAggregateService)
        {
            _clientRepository = clientRepository;
            _scopeFactory = scopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
            _assignAppointmentAggregateService = assignAppointmentAggregateService;
            _updateAppointmentAggregateService = updateAppointmentAggregateService;
            _assignLetterAggregateService = assignLetterAggregateService;
            _updateLetterAggregateService = updateLetterAggregateService;
            _assignPhonecallAggregateService = assignPhonecallAggregateService;
            _updatePhonecallAggregateService = updatePhonecallAggregateService;
            _assignTaskAggregateService = assignTaskAggregateService;
            _updateTaskAggregateService = updateTaskAggregateService;
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

                ChangeRelatedActivities(request.Client.Id, request.AppendedClientId);

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

        private void AssignRelatedActivities(long appendedClientId, long newOwnerCode)
        {
            foreach (var appointment in _appointmentReadModel.LookupOpenAppointmentsRegarding(EntityName.Client, appendedClientId))
            {
                _assignAppointmentAggregateService.Assign(appointment, newOwnerCode);
            }
            foreach (var letter in _letterReadModel.LookupOpenLettersRegarding(EntityName.Client, appendedClientId))
            {
                _assignLetterAggregateService.Assign(letter, newOwnerCode);
            }
            foreach (var phonecall in _phonecallReadModel.LookupOpenPhonecallsRegarding(EntityName.Client, appendedClientId))
            {
                _assignPhonecallAggregateService.Assign(phonecall, newOwnerCode);
            }
            foreach (var task in _taskReadModel.LookupOpenTasksRegarding(EntityName.Client, appendedClientId))
            {
                _assignTaskAggregateService.Assign(task, newOwnerCode);
            }
        }

        private void ChangeRelatedActivities(long newClientId, long appendedClientId)
        {
            foreach (var appointment in _appointmentReadModel.LookupAppointmentsRegarding(EntityName.Client, appendedClientId))
            {
                var regardingObjects = _appointmentReadModel.GetRegardingObjects(appointment.Id).ToList();
                _updateAppointmentAggregateService.ChangeRegardingObjects(
                                                                          appointment,
                                                                          regardingObjects,
                                                                          ReplaceClient<Appointment, AppointmentRegardingObject>(regardingObjects, newClientId));
            }
            foreach (var letter in _letterReadModel.LookupLettersRegarding(EntityName.Client, appendedClientId))
            {
                var regardingObjects = _letterReadModel.GetRegardingObjects(letter.Id).ToList();
                _updateLetterAggregateService.ChangeRegardingObjects(
                                                                     letter,
                                                                     regardingObjects,
                                                                     ReplaceClient<Letter, LetterRegardingObject>(regardingObjects, newClientId));
            }
            foreach (var phonecall in _phonecallReadModel.LookupPhonecallsRegarding(EntityName.Client, appendedClientId))
            {
                var regardingObjects = _phonecallReadModel.GetRegardingObjects(phonecall.Id).ToList();
                _updatePhonecallAggregateService.ChangeRegardingObjects(
                                                                        phonecall,
                                                                        regardingObjects,
                                                                        ReplaceClient<Phonecall, PhonecallRegardingObject>(regardingObjects, newClientId));
            }
            foreach (var task in _taskReadModel.LookupTasksRegarding(EntityName.Client, appendedClientId))
            {
                var regardingObjects = _taskReadModel.GetRegardingObjects(task.Id).ToList();
                _updateTaskAggregateService.ChangeRegardingObjects(task, regardingObjects, ReplaceClient<Task, TaskRegardingObject>(regardingObjects, newClientId));
            }
        }

        private static IEnumerable<TEntityReference> ReplaceClient<TEntity, TEntityReference>(IEnumerable<TEntityReference> regardingObjects, long newClientId)
            where TEntity : class, IEntity, IEntityKey where TEntityReference : EntityReference<TEntity>, new()
        {
            foreach (var regardingObject in regardingObjects)
            {
                if (regardingObject.TargetEntityName == EntityName.Client)
                {
                    yield return new TEntityReference
                                     {
                                         SourceEntityId = regardingObject.SourceEntityId,
                                         TargetEntityName = regardingObject.TargetEntityName, 
                                         TargetEntityId = newClientId
                                     };
                }
                else
                {
                    yield return regardingObject;
                }
            }
        }
    }
}