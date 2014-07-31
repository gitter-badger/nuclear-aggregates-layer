using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetPhonecallDtoService : GetDomainEntityDtoServiceBase<Phonecall>, IRussiaAdapted
    {
		private readonly IActivityReadModel _activityReadModel;
		private readonly IClientReadModel _clientReadModel;
	    private readonly IDealReadModel _dealReadModel;
		private readonly IFirmReadModel _firmReadModel;
		private readonly IUserContext _userContext;

		public GetPhonecallDtoService(IUserContext userContext, IActivityReadModel activityReadModel,
			IClientReadModel clientReadModel, IDealReadModel dealReadModel, IFirmReadModel firmReadModel)
			: base(userContext)
		{
			_activityReadModel = activityReadModel;
			_clientReadModel = clientReadModel;
			_dealReadModel = dealReadModel;
			_firmReadModel = firmReadModel;
			_userContext = userContext;
		}

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _activityReadModel.GetPhonecall(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new PhonecallDomainEntityDto
                {
                    Id = phonecall.Id,
                    AfterSaleServiceType = phonecall.AfterSaleServiceType,
                    ClientRef = new EntityReference { Id = phonecall.ClientId, Name = phonecall.ClientName },
                    ContactRef = new EntityReference { Id = phonecall.ContactId, Name = phonecall.ContactName },
                    DealRef = new EntityReference { Id = phonecall.DealId, Name = phonecall.DealName },
                    Description = phonecall.Description,
                    FirmRef = new EntityReference { Id = phonecall.FirmId, Name = phonecall.FirmName },
                    Header = phonecall.Header,
                    Priority = phonecall.Priority,
                    Purpose = phonecall.Purpose,
                    ScheduledEnd = phonecall.ScheduledEnd.Add(timeOffset),
                    ScheduledStart = phonecall.ScheduledStart.Add(timeOffset),
                    ActualEnd = phonecall.ActualEnd.HasValue ? phonecall.ActualEnd.Value.Add(timeOffset) : phonecall.ActualEnd,
                    Status = phonecall.Status,
                    Type = phonecall.Type,
                    OwnerRef = new EntityReference { Id = phonecall.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = phonecall.CreatedBy, Name = null },
                    CreatedOn = phonecall.CreatedOn,
                    IsActive = phonecall.IsActive,
                    IsDeleted = phonecall.IsDeleted,
                    ModifiedByRef = new EntityReference { Id = phonecall.ModifiedBy, Name = null },
                    ModifiedOn = phonecall.ModifiedOn,
                    Timestamp = phonecall.Timestamp
                };
        }

        protected override IDomainEntityDto<Phonecall> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;
            var dto = new PhonecallDomainEntityDto
                {
                    Type = ActivityType.Phonecall,
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