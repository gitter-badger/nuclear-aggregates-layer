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
    // FIXME {all, 06.11.2013}: �������� �� BL.Operations - ��� ����� � ������ �������, ������ �� ������������ ������ � TFS ��-�� �������������� merge - ���� ��������� ��� �����, ��� RI �� 1.0 ����� �������� �������� ����� ������� ���� ���������� �� 2��
    // ������ ����������� ������� internal, ����� �� ������������� massprocessor
    internal sealed class CzechPhonecallObtainer : IBusinessModelEntityObtainer<Phonecall>, IAggregateReadModel<ActivityBase>, ICzechAdapted
    {
        private readonly IActivityService _activityService;
        private readonly IUserContext _userContext;

        public CzechPhonecallObtainer(IActivityService activityService, IUserContext userContext)
        {
            _activityService = activityService;
            _userContext = userContext;
        }

        public Phonecall ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PhonecallDomainEntityDto)domainEntityDto;

            var phoneCall = dto.Id == 0
                                ? new Phonecall { IsActive = true }
                                : _activityService.GetPhonecall(dto.Id);

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