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
        private readonly IActivityReferenceReader _activityReadModel;        
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public GetTaskDtoService(IUserContext userContext,
                                 ITaskReadModel taskReadModel,
                                 IActivityReferenceReader activityReadModel,
                                 IClientReadModel clientReadModel,
                                 IDealReadModel dealReadModel,
                                 IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _taskReadModel = taskReadModel;
            _activityReadModel = activityReadModel;            
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<Task> GetDto(long entityId)
        {
            var task = _taskReadModel.GetTask(entityId);
            if (task == null)
            {
                throw new InvalidOperationException("The task does not exist for the specified ID.");
            }

            var regardingObjects = _taskReadModel.GetRegardingObjects(entityId);

            return new TaskDomainEntityDto
                {
                    Id = task.Id,
                    Header = task.Header,
                    Description = task.Description,
                    ScheduledOn = task.ScheduledOn,
                    TaskType = task.TaskType,
                    Priority = task.Priority,
                    Status = task.Status,
                    RegardingObjects = AdaptReferences(regardingObjects),

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
            var dto = new TaskDomainEntityDto
                {
                    ScheduledOn = DateTime.Now,
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                };

            EntityReference regardingObject = null;
            if (parentEntityName.CanBeRegardingObject())
            {
                regardingObject = ToEntityReference(parentEntityName, parentEntityId);
            }
            else if (parentEntityName == EntityName.Contact && parentEntityId.HasValue)
            {
                var client = _clientReadModel.GetClientByContact(parentEntityId.Value);
                if (client != null)
                {
                    regardingObject = ToEntityReference(EntityName.Client, client.Id);
                }
            }
            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
            {
                dto.RegardingObjects = _activityReadModel.GetRegardingObjects(parentEntityName, parentEntityId.Value);
            }

            if (regardingObject != null)
            {
                dto.RegardingObjects = new[] { regardingObject };
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Task>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityName entityName, long? entityId)
        {
            if (!entityId.HasValue) return null;

            string name;
            switch (entityName)
            {
                case EntityName.Client:
                    name = _clientReadModel.GetClientName(entityId.Value);
                    break;
                case EntityName.Deal:
                    name = _dealReadModel.GetDeal(entityId.Value).Name;
                    break;
                case EntityName.Firm:
                    name = _firmReadModel.GetFirmName(entityId.Value);
                    break;
                default:
                    return null;
            }

            return new EntityReference { Id = entityId, Name = name, EntityName = entityName };
        }
    }
}