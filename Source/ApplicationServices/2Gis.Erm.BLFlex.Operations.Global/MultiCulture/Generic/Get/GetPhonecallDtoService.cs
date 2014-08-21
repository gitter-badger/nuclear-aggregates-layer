using System;
using System.Linq;

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
    public class GetPhonecallDtoService : GetDomainEntityDtoServiceBase<Phonecall>, ICyprusAdapted, IChileAdapted, ICzechAdapted, IUkraineAdapted, IEmiratesAdapted
    {
        private readonly IActivityReadModel _activityReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IUserContext _userContext;

        public GetPhonecallDtoService(IUserContext userContext,
                                      IActivityReadModel activityReadModel,
                                      IClientReadModel clientReadModel,
                                      IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _activityReadModel = activityReadModel;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _activityReadModel.GetPhonecall(entityId);
            var regardingObjects = _activityReadModel.GetRegardingObjects<Phonecall>(entityId).ToList();

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new PhonecallDomainEntityDto
                {
                    Id = phonecall.Id,
                    CreatedByRef = new EntityReference { Id = phonecall.CreatedBy, Name = null },
                    CreatedOn = phonecall.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = phonecall.ModifiedBy, Name = null },
                    ModifiedOn = phonecall.ModifiedOn,
                    IsActive = phonecall.IsActive,
                    IsDeleted = phonecall.IsDeleted,
                    Timestamp = phonecall.Timestamp,
                    OwnerRef = new EntityReference { Id = phonecall.OwnerCode, Name = null },

                    Header = phonecall.Header,
                    Description = phonecall.Description,
                    ScheduledStart = phonecall.ScheduledStart.Add(timeOffset),
                    ScheduledEnd = phonecall.ScheduledEnd.Add(timeOffset),
                    ActualEnd = phonecall.ActualEnd.HasValue ? phonecall.ActualEnd.Value.Add(timeOffset) : phonecall.ActualEnd,
                    Priority = phonecall.Priority,
                    Status = phonecall.Status,

                    ClientRef = regardingObjects.Lookup(EntityName.Client, _clientReadModel.GetClientName),
                    ContactRef = regardingObjects.Lookup(EntityName.Contact, _clientReadModel.GetContactName),
                    FirmRef = regardingObjects.Lookup(EntityName.Firm, _firmReadModel.GetFirmName),

                    Purpose = phonecall.Purpose,
                    // AfterSaleServiceType = phonecall.AfterSaleServiceType,
                };
        }

        protected override IDomainEntityDto<Phonecall> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var now = DateTime.Now;

            var dto = new PhonecallDomainEntityDto
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