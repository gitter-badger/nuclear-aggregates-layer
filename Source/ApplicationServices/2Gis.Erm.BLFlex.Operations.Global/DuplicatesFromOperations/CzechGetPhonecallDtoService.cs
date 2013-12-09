using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BL.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: �������� �� BL.Operations - ��� ����� � ������ �������, ������ �� ������������ ������ � TFS ��-�� �������������� merge - ���� ��������� ��� �����, ��� RI �� 1.0 ����� �������� �������� ����� ������� ���� ���������� �� 2��
    // ������ ����������� ������� internal, ����� �� ������������� massprocessor
    internal sealed class CzechGetPhonecallDtoService : GetDomainEntityDtoServiceBase<Phonecall>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly IActivityReadModel _activityReadModel;
        private readonly IUserContext _userContext;

        public CzechGetPhonecallDtoService(IUserContext userContext, IFinder finder, IActivityReadModel activityReadModel)
            : base(userContext)
        {
            _finder = finder;
            _activityReadModel = activityReadModel;
            _userContext = userContext;
        }

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _activityReadModel.GetActivity<Phonecall>(entityId);

            var timeOffset = _userContext.Profile != null ? _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo.GetUtcOffset(DateTime.Now) : TimeSpan.Zero;

            return new PhonecallDomainEntityDto
                {
                    Id = phonecall.Id,
                    AfterSaleServiceType = phonecall.AfterSaleServiceType,
                    ClientRef = new EntityReference { Id = phonecall.ClientId, Name = phonecall.ClientName },
                    ContactRef = new EntityReference { Id = phonecall.ContactId, Name = phonecall.ContactName },
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