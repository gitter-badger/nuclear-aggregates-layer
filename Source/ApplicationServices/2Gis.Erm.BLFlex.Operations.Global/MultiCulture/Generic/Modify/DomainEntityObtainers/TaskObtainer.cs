using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Modify.DomainEntityObtainers
{
    public sealed class TaskObtainer : IBusinessModelEntityObtainer<Task>, IAggregateReadModel<Task>, ICyprusAdapted, ICzechAdapted, IChileAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;

        public TaskObtainer(IUserContext userContext, IFinder finder)
        {
            _userContext = userContext;
            _finder = finder;
        }

        public Task ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (TaskDomainEntityDto)domainEntityDto;

            var task = dto.IsNew() ? new Task { IsActive = true } : _finder.FindOne(Specs.Find.ById<Task>(dto.Id));

            // FIXME {s.pomadin, 21.08.2014}: Смещение времени относительно UTC должно быть выполнено на клиентской части
            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            task.TaskType = dto.TaskType;

            task.Description = dto.Description;
            task.Header = dto.Header;
            task.Priority = dto.Priority;
            task.ScheduledStart = dto.ScheduledStart.Subtract(timeOffset);
            task.ScheduledEnd = dto.ScheduledEnd.Subtract(timeOffset);
            task.ActualEnd = dto.ActualEnd.HasValue ? dto.ActualEnd.Value.Subtract(timeOffset) : dto.ActualEnd;
            task.Status = dto.Status;
            task.OwnerCode = dto.OwnerRef.Id.Value;
            task.IsActive = dto.IsActive;
            task.IsDeleted = dto.IsDeleted;

            task.Timestamp = dto.Timestamp;

            return task;
        }
    }
}
