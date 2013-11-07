using System;

using DoubleGis.Erm.BL.Aggregates.Activities;
using DoubleGis.Erm.BL.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: вынесено из BL.Operations - уже копия в данном проекте, похоже на дублирование файлов в TFS из-за многочисленных merge - пока оставлены обе копии, при RI из 1.0 нужно обращать внимание какой целевой файл выбирается из 2ух
    // указан модификатор доступа internal, чтобы не подхватывался massprocessor
    internal class CzechTaskObtainer : IBusinessModelEntityObtainer<Task>, IAggregateReadModel<ActivityBase>, ICzechAdapted
    {
        private readonly IActivityService _activityService;
        private readonly IUserContext _userContext;

        public CzechTaskObtainer(IActivityService activityService, IUserContext userContext)
        {
            _activityService = activityService;
            _userContext = userContext;
        }

        public Task ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TaskDomainEntityDto)domainEntityDto;

            var task = dto.Id == 0
                           ? new Task { IsActive = true }
                           : _activityService.GetTask(dto.Id);

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