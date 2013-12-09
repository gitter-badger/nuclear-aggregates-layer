using System;

using DoubleGis.Erm.BL.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public sealed class TaskObtainer : IBusinessModelEntityObtainer<Task>, IAggregateReadModel<ActivityBase>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public TaskObtainer(IUserContext userContext, IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
        {
            _userContext = userContext;
            _finder = finder;
            _activityDynamicPropertiesConverter = activityDynamicPropertiesConverter;
        }

        public Task ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TaskDomainEntityDto)domainEntityDto;

            var task = dto.IsNew() ? new Task { IsActive = true } : _finder.Single<Task>(dto.Id, _activityDynamicPropertiesConverter);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;
            
            task.TaskType = dto.TaskType;

            task.ClientId = dto.ClientRef.Id;
            task.ContactId = dto.ContactRef.Id;
            task.DealId = dto.DealRef.Id;
            task.Description = dto.Description;
            task.FirmId = dto.FirmRef.Id;
            task.Header = dto.Header;
            task.Priority = dto.Priority;
            task.ScheduledStart = dto.ScheduledStart.Subtract(timeOffset);
            task.ScheduledEnd = dto.ScheduledEnd.Subtract(timeOffset);
            task.ActualEnd = dto.ActualEnd.HasValue ? dto.ActualEnd.Value.Subtract(timeOffset) : dto.ActualEnd;
            task.Status = dto.Status;
            task.Type = dto.Type;
            task.OwnerCode = dto.OwnerRef.Id.Value;
            task.IsActive = dto.IsActive;
            task.IsDeleted = dto.IsDeleted;

            task.Timestamp = dto.Timestamp;

            return task;
        }
    }
}
