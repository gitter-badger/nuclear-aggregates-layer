﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetTaskDtoService : GetDomainEntityDtoServiceBase<Task>, IRussiaAdapted
    {
		private readonly IUserContext _userContext;
		private readonly IActivityReadModel _activityReadModel;
		private readonly IClientReadModel _clientReadModel;
	    private readonly IDealReadModel _dealReadModel;
		private readonly IFirmReadModel _firmReadModel;

		public GetTaskDtoService(IUserContext userContext, IActivityReadModel activityReadModel,
			IClientReadModel clientReadModel, IDealReadModel dealReadModel, IFirmReadModel firmReadModel)
			: base(userContext)
		{
			_userContext = userContext;
			_activityReadModel = activityReadModel;
			_clientReadModel = clientReadModel;
			_dealReadModel = dealReadModel;
			_firmReadModel = firmReadModel;
		}

        protected override IDomainEntityDto<Task> GetDto(long entityId)
        {
            var task = _activityReadModel.GetTask(entityId);
			var regardingObjects = _activityReadModel.GetRegardingObjects<Task>(entityId).ToList();

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

					ClientRef = regardingObjects.Lookup(EntityName.Client, _clientReadModel.GetClientName),
					ContactRef = regardingObjects.Lookup(EntityName.Contact, _clientReadModel.GetContactName),
					DealRef = regardingObjects.Lookup(EntityName.Deal, id => _dealReadModel.GetDeal(id).Name),
					FirmRef = regardingObjects.Lookup(EntityName.Firm, _firmReadModel.GetFirmName),

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
				case EntityName.Deal:
					dto.DealRef = new EntityReference
					{
						Id = parentEntityId,
						Name = _dealReadModel.GetDeal(parentEntityId.Value).Name
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