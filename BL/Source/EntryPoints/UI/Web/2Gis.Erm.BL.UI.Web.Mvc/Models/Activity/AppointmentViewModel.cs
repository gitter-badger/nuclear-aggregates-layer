using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public sealed class AppointmentViewModel : EntityViewModelBase<Appointment>, ITitleAspect, IActivityStateAspect
    {
        public override byte[] Timestamp { get; set; }
        
        public override bool IsSecurityRoot { get { return true; } }

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
        public AppointmentPurpose Purpose { get; set; }

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
            Client = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Client().Id));
            Deal = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Deal().Id));
            Firm = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Firm().Id));

            Attendee = LookupField.FromReference((modelDto.Attendees ?? Enumerable.Empty<EntityReference>()).FirstOrDefault(x => x.EntityTypeId.Equals(EntityType.Instance.Contact())));

            
            // NOTE: Owner, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsActive, IsDeleted and Timestamp fields are set in CreateOrUpdateController.GetViewModel
            // TODO: should it be only there?
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            Action<IList<EntityReference>, LookupField, IEntityType> addIfSet = 
                (references, field, entityName) =>
                {
                    if (field.Key.HasValue)
                    {
                        references.Add(new EntityReference(field.Key, field.Value) { EntityTypeId = entityName.Id });
                    }
                };

            var regardingObjects = new List<EntityReference>();
            addIfSet(regardingObjects, Client, EntityType.Instance.Client());
            addIfSet(regardingObjects, Deal, EntityType.Instance.Deal());
            addIfSet(regardingObjects, Firm, EntityType.Instance.Firm());

            var attendees = new List<EntityReference>();
            addIfSet(attendees, Attendee, EntityType.Instance.Contact());

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
                    OwnerRef = Owner.ToReference(EntityType.Instance.User()),
                    Organizer = Owner.ToReference(EntityType.Instance.User()),
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