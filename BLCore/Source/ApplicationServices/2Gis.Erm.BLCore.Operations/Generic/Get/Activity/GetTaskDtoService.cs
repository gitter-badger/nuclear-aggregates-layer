using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetTaskDtoService : GetDomainEntityDtoServiceBase<Task>
    {
        private readonly ITaskReadModel _taskReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRegardingObjects;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRecipients;

        public GetTaskDtoService(IUserContext userContext,
                                 IAppointmentReadModel appointmentReadModel,
                                 IClientReadModel clientReadModel,
                                 IFirmReadModel firmReadModel,
                                 IDealReadModel dealReadModel,
                                 ILetterReadModel letterReadModel,
                                 IPhonecallReadModel phonecallReadModel,
                                 ITaskReadModel taskReadModel)
            : base(userContext)
        {
            _taskReadModel = taskReadModel;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;

            var service = new ActivityReferenceReader(clientReadModel, dealReadModel, firmReadModel);

            _lookupsForRegardingObjects = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Task, entityId => taskReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, entityId => service.ResolveRegardingObjectsFromClient(entityId) },
                { EntityName.Contact, entityId => service.ResolveRegardingObjectsFromContact(entityId) },
                { EntityName.Deal, entityId => service.ResolveRegardingObjectsFromDeal(entityId) },
                { EntityName.Firm, entityId => service.ResolveRegardingObjectsFromFirm(entityId) },
            };

            _lookupsForRecipients = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetAttendees(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, entityId => service.ResolveContactsFromClient(entityId) },
                { EntityName.Contact, entityId => service.ResolveContactsFromContact(entityId) },
                { EntityName.Deal, entityId => service.ResolveContactsFromDeal(entityId) },
                { EntityName.Firm, entityId => service.ResolveContactsFromFirm(entityId) },
            };
        }

        protected override IDomainEntityDto<Task> GetDto(long entityId)
        {
            var task = _taskReadModel.GetTask(entityId);
            if (task == null)
            {
                throw new InvalidOperationException("The task does not exist for the specified ID.");
            }

            return new TaskDomainEntityDto
                {
                    Id = task.Id,
                    Header = task.Header,
                    Description = task.Description,
                    ScheduledOn = task.ScheduledOn,
                    TaskType = task.TaskType,
                    Priority = task.Priority,
                    Status = task.Status,
                    RegardingObjects = _lookupsForRecipients.LookupElements(EntityName.Task, entityId),

                    OwnerRef = new EntityReference { Id = task.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = task.CreatedBy, Name = null },
                    CreatedOn = task.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = task.ModifiedBy, Name = null },
                    ModifiedOn = task.ModifiedOn,
                    IsActive = task.IsActive,
                    IsDeleted = task.IsDeleted,
                    Timestamp = task.Timestamp,
                };
        }

        protected override IDomainEntityDto<Task> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new TaskDomainEntityDto
                       {
                           ScheduledOn = DateTime.Now,
                           Priority = ActivityPriority.Average,
                           Status = ActivityStatus.InProgress,

                           RegardingObjects = _lookupsForRecipients.LookupElements(parentEntityName, parentEntityId),
                       };
        }

        private EntityReference EmbedEntityNameIfNeeded(EntityReference reference)
        {
            if (reference.Id != null && reference.Name == null)
            {
                reference.Name = ReadEntityName(reference.EntityName, reference.Id.Value);
            }
            return reference;
        }

        private string ReadEntityName(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return _clientReadModel.GetClientName(entityId);
                case EntityName.Contact:
                    return _clientReadModel.GetContactName(entityId);
                case EntityName.Deal:
                    return _dealReadModel.GetDeal(entityId).Name;
                case EntityName.Firm:
                    return _firmReadModel.GetFirmName(entityId);
                default:
                    throw new ArgumentOutOfRangeException("entityName");
            }
        }
    }
}