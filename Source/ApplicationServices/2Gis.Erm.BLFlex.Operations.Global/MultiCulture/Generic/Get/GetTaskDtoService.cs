using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Get
{
    public class GetTaskDtoService : GetDomainEntityDtoServiceBase<Task>, ICyprusAdapted, IChileAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IActivityReadModel _activityReadModel;
	    private readonly IClientReadModel _clientReadModel;
	    private readonly IFirmReadModel _firmReadModel;
	    private readonly IUserContext _userContext;

		public GetTaskDtoService(IUserContext userContext, IActivityReadModel activityReadModel, 
			IClientReadModel clientReadModel, IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _activityReadModel = activityReadModel;
	        _clientReadModel = clientReadModel;
			_firmReadModel = firmReadModel;
	        _userContext = userContext;
        }

        protected override IDomainEntityDto<Task> GetDto(long entityId)
        {
            var task = _activityReadModel.GetTask(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new TaskDomainEntityDto
                {
					Id = task.Id,
					CreatedByRef = new EntityReference { Id = task.CreatedBy, Name = null },
					CreatedOn = task.CreatedOn,
					ModifiedByRef = new EntityReference { Id = task.ModifiedBy, Name = null },
					ModifiedOn = task.ModifiedOn,
					IsActive = task.IsActive,
					IsDeleted = task.IsDeleted,
					Timestamp = task.Timestamp,
					OwnerRef = new EntityReference { Id = task.OwnerCode, Name = null },

					Header = task.Header,
					Description = task.Description,
					ScheduledStart = task.ScheduledStart.Add(timeOffset),
					ScheduledEnd = task.ScheduledEnd.Add(timeOffset),
					ActualEnd = task.ActualEnd.HasValue ? task.ActualEnd.Value.Add(timeOffset) : task.ActualEnd,
					Priority = task.Priority,
					Status = task.Status,

					ClientRef = task.RegardingObjects.Lookup(ReferenceType.RegardingObject, EntityName.Client, _clientReadModel.GetClientName),
					ContactRef = task.RegardingObjects.Lookup(ReferenceType.RegardingObject, EntityName.Contact, _clientReadModel.GetContactName),
					FirmRef = task.RegardingObjects.Lookup(ReferenceType.RegardingObject, EntityName.Firm, _firmReadModel.GetFirmName),

					TaskType = task.TaskType,
				};
        }

        protected override IDomainEntityDto<Task> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;

            var dto = new TaskDomainEntityDto
                {
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
						Name = _clientReadModel.GetClientName(parentEntityId.Value)
					};
					break;
				case EntityName.Contact:
					dto.ContactRef = new EntityReference
					{
						Id = parentEntityId,
						Name = _clientReadModel.GetContactName(parentEntityId.Value)
					};
					break;
				case EntityName.Firm:
					dto.FirmRef = new EntityReference
					{
						Id = parentEntityId,
						Name = _firmReadModel.GetFirmName(parentEntityId.Value)
					};
					break;
			}

            return dto;
        }
    }
}