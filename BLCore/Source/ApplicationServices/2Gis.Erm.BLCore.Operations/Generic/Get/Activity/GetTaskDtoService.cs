using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
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
        private readonly IActivityReferenceReader _activityReferenceReader;

        public GetTaskDtoService(IUserContext userContext,
                                 ITaskReadModel taskReadModel,
                                 IActivityReferenceReader activityReferenceReader)
            : base(userContext)
        {
            _taskReadModel = taskReadModel;
            _activityReferenceReader = activityReferenceReader;
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

            if (parentEntityName.CanBeRegardingObject() || parentEntityName.CanBeContacted())
            {
                var regardingObject = _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId);
                if (regardingObject.Id != null)
                {
                    dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(regardingObject);
                }
            }
            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
            {
                dto.RegardingObjects = _activityReferenceReader.GetRegardingObjects(parentEntityName, parentEntityId.Value);
            }
            
            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Task>> references)
        {
            return references.Select(x => _activityReferenceReader.ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }
    }
}