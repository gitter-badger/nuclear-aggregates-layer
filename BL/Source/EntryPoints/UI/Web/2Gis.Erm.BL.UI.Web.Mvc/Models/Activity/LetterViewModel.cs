﻿using System;
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
    public sealed class LetterViewModel : EntityViewModelBase<Letter>, ITitleAspect, IActivityStateAspect
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

        public bool FirmClientInitialization { get; set; }
        public bool DealClientInitialization { get; set; }
        public bool RecipientClientInitialization { get; set; }

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
            Client = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Client().Id));
            Deal = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Deal().Id));
            Firm = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityTypeId == EntityType.Instance.Firm().Id));

            Sender = LookupField.FromReference(modelDto.SenderRef);
            Recipient = LookupField.FromReference(modelDto.RecipientRef);

            FirmClientInitialization = regardingObjects.IsClientInitialization(EntityType.Instance.Firm().Id);
            DealClientInitialization = regardingObjects.IsClientInitialization(EntityType.Instance.Deal().Id);
            RecipientClientInitialization = modelDto.RecipientRef.IsClientInitialization(EntityType.Instance.Contact().Id);

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

            return new LetterDomainEntityDto
                {
                    Id = Id,
                    Priority = Priority,
                    Status = Status,
                    Header = Title,
                    Description = Description,
                    ScheduledOn = ScheduledStart,
                    RegardingObjects = regardingObjects,
                    SenderRef = Sender.ToReference(EntityType.Instance.User()),
                    RecipientRef = Recipient.ToReference(EntityType.Instance.Contact()),
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