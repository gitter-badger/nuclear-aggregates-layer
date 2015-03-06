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
    public sealed class LetterViewModel : EntityViewModelBase<Letter>, IActivityViewModel
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
        [StringLengthLocalized(256)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Calendar, JsonConverter(typeof(IsoDateTimeConverter)), RequiredLocalized]
        public DateTime ScheduledStart { get; set; }

        public LookupField Client { get; set; }
        public LookupField Deal { get; set; }
        public LookupField Firm { get; set; }
        public LookupField Sender { get; set; }
        public LookupField Recipient { get; set; }

        public bool FirmAutoUpdate { get; set; }
        public bool DealAutoUpdate { get; set; }
        public bool RecipientAutoUpdate { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LetterDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Title = modelDto.Header;
            Description = modelDto.Description;
            ScheduledStart = modelDto.ScheduledOn.UpdateKindIfUnset();
            Priority = modelDto.Priority;
            Status = modelDto.Status;

            var regardingObjects = (modelDto.RegardingObjects ?? Enumerable.Empty<EntityReference>()).ToList();
            Client = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Client));
            Deal = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Deal));
            Firm = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Firm));

            Sender = LookupField.FromReference(modelDto.SenderRef);
            Recipient = LookupField.FromReference(modelDto.RecipientRef);

            FirmAutoUpdate = regardingObjects.IsAutoUpdate(EntityName.Firm);
            DealAutoUpdate = regardingObjects.IsAutoUpdate(EntityName.Deal);
            RecipientAutoUpdate = modelDto.RecipientRef.IsAutoUpdate(EntityName.Contact);

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

            return new LetterDomainEntityDto
                {
                    Id = Id,
                    Priority = Priority,
                    Status = Status,
                    Header = Title,
                    Description = Description,
                    ScheduledOn = ScheduledStart,
                    RegardingObjects = regardingObjects,
                    SenderRef = Sender.ToReference(EntityName.User),
                    RecipientRef = Recipient.ToReference(EntityName.Contact),
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