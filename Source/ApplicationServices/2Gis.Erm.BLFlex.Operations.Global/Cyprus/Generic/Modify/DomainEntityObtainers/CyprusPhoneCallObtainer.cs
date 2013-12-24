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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.DomainEntityObtainers
{
    public sealed class CyprusPhonecallObtainer : IBusinessModelEntityObtainer<Phonecall>, IAggregateReadModel<ActivityBase>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly IActivityDynamicPropertiesConverter _activityDynamicPropertiesConverter;

        public CyprusPhonecallObtainer(IUserContext userContext, IFinder finder, IActivityDynamicPropertiesConverter activityDynamicPropertiesConverter)
        {
            _userContext = userContext;
            _finder = finder;
            _activityDynamicPropertiesConverter = activityDynamicPropertiesConverter;
        }

        public Phonecall ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PhonecallDomainEntityDto)domainEntityDto;

            var phoneCall = dto.IsNew() ? new Phonecall { IsActive = true } : _finder.Single<Phonecall>(dto.Id, _activityDynamicPropertiesConverter);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            phoneCall.ClientId = dto.ClientRef.Id;
            phoneCall.ContactId = dto.ContactRef.Id;
            phoneCall.Description = dto.Description;
            phoneCall.FirmId = dto.FirmRef.Id;
            phoneCall.Header = dto.Header;
            phoneCall.Priority = dto.Priority;
            phoneCall.Purpose = dto.Purpose;
            phoneCall.ScheduledStart = dto.ScheduledStart.Subtract(timeOffset);
            phoneCall.ScheduledEnd = dto.ScheduledEnd.Subtract(timeOffset);
            phoneCall.ActualEnd = dto.ActualEnd.HasValue ? dto.ActualEnd.Value.Subtract(timeOffset) : dto.ActualEnd;
            phoneCall.Status = dto.Status;
            phoneCall.Type = dto.Type;
            phoneCall.OwnerCode = dto.OwnerRef.Id.Value;
            phoneCall.IsActive = dto.IsActive;
            phoneCall.IsDeleted = dto.IsDeleted;

            phoneCall.Timestamp = dto.Timestamp;

            return phoneCall;
        }
    }
}
