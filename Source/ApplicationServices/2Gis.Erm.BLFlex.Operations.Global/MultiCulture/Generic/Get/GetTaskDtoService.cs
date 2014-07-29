﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetTaskDtoService : GetDomainEntityDtoServiceBase<Task>, ICyprusAdapted, IChileAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IFinder _finder;
        private readonly IActivityReadModel _activityReadModel;
        private readonly IUserContext _userContext;

        public GetTaskDtoService(IUserContext userContext, IFinder finder, IActivityReadModel activityReadModel)
            : base(userContext)
        {
            _finder = finder;
            _activityReadModel = activityReadModel;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Task> GetDto(long entityId)
        {
            var task = _activityReadModel.GetTask(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new TaskDomainEntityDto
                {
                    Id = task.Id,
                    TaskType = task.TaskType,
                    ClientRef = new EntityReference { Id = task.ClientId, Name = task.ClientName },
                    ContactRef = new EntityReference { Id = task.ContactId, Name = task.ContactName },
                    Description = task.Description,
                    FirmRef = new EntityReference { Id = task.FirmId, Name = task.FirmName },
                    Header = task.Header,
                    Priority = task.Priority,
                    ScheduledEnd = task.ScheduledEnd.Add(timeOffset),
                    ScheduledStart = task.ScheduledStart.Add(timeOffset),
                    ActualEnd = task.ActualEnd.HasValue ? task.ActualEnd.Value.Add(timeOffset) : task.ActualEnd,
                    Status = task.Status,
                    Type = task.Type,
                    OwnerRef = new EntityReference { Id = task.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = task.CreatedBy, Name = null },
                    CreatedOn = task.CreatedOn,
                    IsActive = task.IsActive,
                    IsDeleted = task.IsDeleted,
                    ModifiedByRef = new EntityReference { Id = task.ModifiedBy, Name = null },
                    ModifiedOn = task.ModifiedOn,
                    Timestamp = task.Timestamp
                };
        }

        protected override IDomainEntityDto<Task> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;

            var dto = new TaskDomainEntityDto
                {
                    Type = ActivityType.Task,
                    IsActive = true,
                    ScheduledStart = now,
                    ScheduledEnd = now.Add(TimeSpan.FromMinutes(15.0)),
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                };

            if (parentEntityId == null)
            {
                return dto;
            }

            switch (parentEntityName)
            {
                case EntityName.Client:
                    dto.ClientRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _finder.Find(Specs.Find.ById<Client>(parentEntityId.Value)).Select(x => x.Name).Single()
                        };
                    break;
                case EntityName.Firm:
                    dto.FirmRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _finder.Find(Specs.Find.ById<Firm>(parentEntityId.Value)).Select(x => x.Name).Single()
                        };
                    break;
                case EntityName.Contact:
                    dto.ContactRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _finder.Find(Specs.Find.ById<Contact>(parentEntityId.Value)).Select(x => x.FullName).Single()
                        };
                    break;
            }

            return dto;
        }
    }
}