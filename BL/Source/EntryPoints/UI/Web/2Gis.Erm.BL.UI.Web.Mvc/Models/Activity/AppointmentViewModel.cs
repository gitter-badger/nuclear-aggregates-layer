using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public sealed class AppointmentViewModel : EntityViewModelBase<Appointment>, IActivityViewModel
    {
        public override byte[] Timestamp { get; set; }

        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        public override string EntityStatus
        {
            get
            {
                return Status.ToStringLocalized(EnumResources.ResourceManager, null);
            }
        }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public ActivityStatus Status { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public ActivityPriority Priority { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public ActivityPurpose Purpose { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Calendar, JsonConverter(typeof(IsoDateTimeConverter)), RequiredLocalized]
        public DateTime ScheduledStart { get; set; }

        [Calendar, JsonConverter(typeof(IsoDateTimeConverter)), RequiredLocalized]
        public DateTime ScheduledEnd { get; set; }

        public LookupField Client { get; set; }
        public LookupField Deal { get; set; }
        public LookupField Firm { get; set; }
        public LookupField Attendee { get; set; }

        [StringLengthLocalized(256)]
        public string Location { get; set; }

        public IEnumerable<string> AmbiguousLookupFields { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (AppointmentDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Title = modelDto.Header;
            Description = modelDto.Description;
            ScheduledStart = modelDto.ScheduledStart.UpdateKindIfUnset();
            ScheduledEnd = modelDto.ScheduledEnd.UpdateKindIfUnset();
            Priority = modelDto.Priority;
            Purpose = modelDto.Purpose;
            Status = modelDto.Status;
            Location = modelDto.Location;

            var regardingObjects = (modelDto.RegardingObjects ?? Enumerable.Empty<EntityReference>()).ToList();
            Client = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Client));
            Deal = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Deal));
            Firm = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Firm));

            var attendeeReference = (modelDto.Attendees ?? Enumerable.Empty<EntityReference>()).FirstOrDefault(x => x.EntityName == EntityName.Contact);
            Attendee = LookupField.FromReference(attendeeReference);
            

            AmbiguousLookupFields = (modelDto.Attendees != null ? modelDto.Attendees.Concat(regardingObjects) : regardingObjects).GetAmbiguousFields();             

            // NOTE: Owner, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsActive, IsDeleted and Timestamp fields are set in CreateOrUpdateController.GetViewModel
            // TODO: should it be only there?
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            Action<IList<EntityReference>, LookupField, EntityName> addIfSet = 
                (references, field, entityName) =>
                {
                    if (field.Key.HasValue)
                    {
                        references.Add(new EntityReference(field.Key, field.Value) { EntityName = entityName });
                    }
                };

            var regardingObjects = new List<EntityReference>();
            addIfSet(regardingObjects, Client, EntityName.Client);
            addIfSet(regardingObjects, Deal, EntityName.Deal);
            addIfSet(regardingObjects, Firm, EntityName.Firm);

            var attendees = new List<EntityReference>();
            addIfSet(attendees, Attendee, EntityName.Contact);

            return new AppointmentDomainEntityDto
                {
                    Id = Id,
                    Header = Title,
                    Description = Description,
                    ScheduledStart = ScheduledStart,
                    ScheduledEnd = ScheduledEnd,
                    Priority = Priority,
                    Purpose = Purpose,
                    Status = Status,
                    Location = Location,
                    RegardingObjects = regardingObjects,
                    Attendees = attendees,
                    OwnerRef = Owner.ToReference(),

                    CreatedByRef = CreatedBy.ToReference(),
                    CreatedOn = CreatedOn,
                    ModifiedByRef = ModifiedBy.ToReference(),
                    ModifiedOn = ModifiedOn,
                    IsActive = IsActive,
                    IsDeleted = IsDeleted,
                    Timestamp = Timestamp,
                };
        }
    }
}