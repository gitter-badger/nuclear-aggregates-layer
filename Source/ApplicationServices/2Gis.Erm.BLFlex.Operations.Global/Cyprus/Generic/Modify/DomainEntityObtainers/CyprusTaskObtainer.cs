using System;

using DoubleGis.Erm.BLCore.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.DomainEntityObtainers
{
    public sealed class CyprusTaskObtainer : IBusinessModelEntityObtainer<Task>, IAggregateReadModel<ActivityBase>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public CyprusTaskObtainer(IUserContext userContext, IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
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
