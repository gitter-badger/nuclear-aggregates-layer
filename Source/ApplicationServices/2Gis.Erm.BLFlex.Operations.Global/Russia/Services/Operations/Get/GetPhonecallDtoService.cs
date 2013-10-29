using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Activities;
using DoubleGis.Erm.BL.Operations.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BL.Services.Operations.Get
{
    public class GetPhonecallDtoService : GetDomainEntityDtoServiceBase<Phonecall>, IRussiaAdapted
    {
        private readonly IFinder _finder;
        private readonly IActivityService _activityService;
        private readonly IUserContext _userContext;

        public GetPhonecallDtoService(IUserContext userContext, IFinder finder, IActivityService activityService)
            : base(userContext)
        {
            _finder = finder;
            _activityService = activityService;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _activityService.GetPhonecall(entityId);

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
                            Name = _finder.Find(GenericSpecifications.ById<Client>(parentEntityId.Value)).Select(x => x.Name).Single()
                        };
                    break;
                case EntityName.Deal:
                    dto.DealRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _finder.Find(GenericSpecifications.ById<Deal>(parentEntityId.Value)).Select(x => x.Name).Single()
                        };
                    break;
                case EntityName.Firm:
                    dto.FirmRef = new EntityReference
                    {
                        Id = parentEntityId,
                        Name = _finder.Find(GenericSpecifications.ById<Firm>(parentEntityId.Value)).Select(x => x.Name).Single()
                    };
                    break;
                case EntityName.Contact:
                    dto.ContactRef = new EntityReference
                        {
                            Id = parentEntityId,
                            Name = _finder.Find(GenericSpecifications.ById<Contact>(parentEntityId.Value)).Select(x => x.FullName).Single()
                        };
                    break;
            }

            return dto;
        }
    }
}