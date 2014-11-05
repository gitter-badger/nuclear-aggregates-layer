using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureAppointmentViewModel : ActivityBaseViewModelAbstract<Appointment>, 
                                                           ICyprusAdapted, 
                                                           IChileAdapted, 
                                                           ICzechAdapted, 
                                                           IUkraineAdapted, 
                                                           IEmiratesAdapted, 
                                                           IKazakhstanAdapted
    {
        public MultiCultureAppointmentViewModel()
            : base(ActivityType.Appointment)
        {
        }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Purpose", "this.value != 'NotSet'")]
        public ActivityPurpose Purpose { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (AppointmentDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Priority = modelDto.Priority;
            Status = modelDto.Status;
            Purpose = modelDto.Purpose;
            Header = modelDto.Header;
            ScheduledStart = modelDto.ScheduledStart;
            ScheduledStartTime = modelDto.ScheduledStart.TimeOfDay;
            ScheduledEnd = modelDto.ScheduledEnd;
            ScheduledEndTime = modelDto.ScheduledEnd.TimeOfDay;
            ActualEnd = modelDto.ActualEnd.HasValue ? modelDto.ActualEnd.Value : (DateTime?)null;
            ActualEndTime = modelDto.ActualEnd.HasValue ? modelDto.ActualEnd.Value.TimeOfDay : TimeSpan.Zero;
            Description = modelDto.Description;

            Client = LookupField.FromReference(modelDto.ClientRef);
            Firm = LookupField.FromReference(modelDto.FirmRef);
            Contact = LookupField.FromReference(modelDto.ContactRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AppointmentDomainEntityDto
                       {
                           Id = Id, 
                           Priority = Priority, 
                           Status = Status, 
                           Purpose = Purpose, 
                           Header = Header, 
                           ScheduledStart = new DateTime(ScheduledStart.Year, ScheduledStart.Month, ScheduledStart.Day).Add(ScheduledStartTime), 
                           ScheduledEnd = new DateTime(ScheduledEnd.Year, ScheduledEnd.Month, ScheduledEnd.Day).Add(ScheduledEndTime), 
                           ActualEnd =
                               ActualEnd.HasValue ? new DateTime(ActualEnd.Value.Year, ActualEnd.Value.Month, ActualEnd.Value.Day).Add(ActualEndTime.Value) : (DateTime?)null, 
                           Description = Description, 
                           ClientRef = Client.ToReference(), 
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