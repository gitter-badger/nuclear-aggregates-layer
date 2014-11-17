﻿using System;
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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity
{
    public sealed class TaskViewModel : EntityViewModelBase<Task>, IActivityViewModel
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
        public TaskType TaskType { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public ActivityPriority Priority { get; set; }

        [RequiredLocalized]
        public string Title { get; set; }

        public string Description { get; set; }

        [Calendar, RequiredLocalized]
        public DateTime ScheduledStart { get; set; }

        public LookupField Client { get; set; }
        public LookupField Deal { get; set; }
        public LookupField Firm { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (TaskDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Status = modelDto.Status;
            TaskType = modelDto.TaskType;
            Priority = modelDto.Priority;
            Title = modelDto.Header;
            Description = modelDto.Description;
            ScheduledStart = modelDto.ScheduledOn.UpdateKindIfUnset().Date;

            var regardingObjects = (modelDto.RegardingObjects ?? Enumerable.Empty<EntityReference>()).ToList();
            Client = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Client));
            Deal = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Deal));
            Firm = LookupField.FromReference(regardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Firm));

            // NOTE: Owner, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn, IsActive, IsDeleted and Timestamp fields are set in CreateOrUpdateController.GetViewModel
            // TODO: should it be it there anyway?
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

            return new TaskDomainEntityDto
                {
                    Id = Id,
                    Status = Status,
                    TaskType = TaskType,
                    Priority = Priority,
                    Header = Title,
                    ScheduledOn = ScheduledStart,
                    Description = Description,
                    RegardingObjects = regardingObjects,
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