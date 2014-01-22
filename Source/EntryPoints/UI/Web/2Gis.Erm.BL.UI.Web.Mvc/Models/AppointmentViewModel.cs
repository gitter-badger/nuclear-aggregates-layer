using System;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AppointmentViewModel : ActivityBaseViewModelAbstract<Appointment>, IRussiaAdapted
    {
        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Purpose", "this.value != 'NotSet'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "AfterSaleServiceType", "this.value != 'Service' && this.value != 'Prolongation'")]
        public ActivityPurpose Purpose { get; set; }

        public AfterSaleServiceType AfterSaleServiceType { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (AppointmentDomainEntityDto)domainEntityDto;
            
            Id = modelDto.Id;
            Type = modelDto.Type;
            Priority = modelDto.Priority;
            Status = modelDto.Status;
            Purpose = modelDto.Purpose;
            AfterSaleServiceType = (AfterSaleServiceType)modelDto.AfterSaleServiceType;
            Header = modelDto.Header;
            ScheduledStart = modelDto.ScheduledStart;
            ScheduledStartTime = modelDto.ScheduledStart.TimeOfDay;
            ScheduledEnd = modelDto.ScheduledEnd;
            ScheduledEndTime = modelDto.ScheduledEnd.TimeOfDay;
            ActualEnd = modelDto.ActualEnd.HasValue ? modelDto.ActualEnd.Value : (DateTime?)null;
            ActualEndTime = modelDto.ActualEnd.HasValue ? modelDto.ActualEnd.Value.TimeOfDay : TimeSpan.Zero;
            Description = modelDto.Description;

            Client = LookupField.FromReference(modelDto.ClientRef);
            Deal = LookupField.FromReference(modelDto.DealRef);
            Firm = LookupField.FromReference(modelDto.FirmRef);
            Contact = LookupField.FromReference(modelDto.ContactRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AppointmentDomainEntityDto
                {
                    Id = Id,
                    Type = Type,
                    Priority = Priority,
                    Status = Status,
                    Purpose = Purpose,
                    AfterSaleServiceType = (byte)AfterSaleServiceType,
                    Header = Header,
                    ScheduledStart = new DateTime(ScheduledStart.Year, ScheduledStart.Month, ScheduledStart.Day).Add(ScheduledStartTime),
                    ScheduledEnd = new DateTime(ScheduledEnd.Year, ScheduledEnd.Month, ScheduledEnd.Day).Add(ScheduledEndTime),
                    ActualEnd = ActualEnd.HasValue ? new DateTime(ActualEnd.Value.Year, ActualEnd.Value.Month, ActualEnd.Value.Day).Add(ActualEndTime.Value) : (DateTime?)null,
                    Description = Description,
                    ClientRef = Client.ToReference(),
                    DealRef = Deal.ToReference(),
                    FirmRef = Firm.ToReference(),
                    ContactRef = Contact.ToReference(),
                    Timestamp = Timestamp,
                    IsActive = IsActive,
                    IsDeleted = IsDeleted,
                    OwnerRef = Owner.ToReference()
                };
        }
    }
}