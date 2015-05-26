using System;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetTaskDtoService : GetActivityDtoService<Task>
    {
        private readonly ITaskReadModel _taskReadModel;

        public GetTaskDtoService(IUserContext userContext,
                                 IAppointmentReadModel appointmentReadModel,
                                 IClientReadModel clientReadModel,
                                 IFirmReadModel firmReadModel,
                                 IDealReadModel dealReadModel,
                                 ILetterReadModel letterReadModel,
                                 IPhonecallReadModel phonecallReadModel,
                                 ITaskReadModel taskReadModel)
            : base(userContext, appointmentReadModel, clientReadModel, firmReadModel, dealReadModel, letterReadModel, phonecallReadModel, taskReadModel)
        {
            _taskReadModel = taskReadModel;
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
                    RegardingObjects = GetRegardingObjects(EntityType.Instance.Task(), entityId),

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
        
        protected override IDomainEntityDto<Task> CreateDto(long? parentEntityId, IEntityType parentEntityType, string extendedInfo)
        {
            return new TaskDomainEntityDto
                       {
                           ScheduledOn = DateTime.Now,
                           Priority = ActivityPriority.Average,
                           Status = ActivityStatus.InProgress,

                           RegardingObjects = GetRegardingObjects(parentEntityType, parentEntityId),
                       };
        }
    }
}