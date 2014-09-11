﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPhonecallDtoService : GetDomainEntityDtoServiceBase<Phonecall>
    {
        private readonly IPhonecallReadModel _activityReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public GetPhonecallDtoService(IUserContext userContext,
                                      IPhonecallReadModel activityReadModel,
                                      IClientReadModel clientReadModel,
                                      IDealReadModel dealReadModel,
                                      IFirmReadModel firmReadModel)
            : base(userContext)
        {
            _activityReadModel = activityReadModel;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
        }

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _activityReadModel.GetPhonecall(entityId);
            if (phonecall == null)
            {
                throw new InvalidOperationException("The phonecall does not exist for the specified ID.");
            }

            var regardingObjects = _activityReadModel.GetRegardingObjects(entityId);
            var recipient = _activityReadModel.GetRecipient(entityId);

            return new PhonecallDomainEntityDto
                {
                    Id = phonecall.Id,
                    Header = phonecall.Header,
                    Description = phonecall.Description,
                    ScheduledOn = phonecall.ScheduledStart,
                    Priority = phonecall.Priority,
                    Purpose = phonecall.Purpose,
                    Status = phonecall.Status,
                    RegardingObjects = AdaptReferences(regardingObjects),
                    RecipientRef = recipient != null ? ToEntityReference(recipient.TargetEntityName, recipient.TargetEntityId) : null,

                    OwnerRef = new EntityReference { Id = phonecall.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = phonecall.CreatedBy, Name = null },
                    CreatedOn = phonecall.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = phonecall.ModifiedBy, Name = null },
                    ModifiedOn = phonecall.ModifiedOn,
                    IsActive = phonecall.IsActive,
                    IsDeleted = phonecall.IsDeleted,
                    Timestamp = phonecall.Timestamp,
                };
        }

        protected override IDomainEntityDto<Phonecall> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var dto = new PhonecallDomainEntityDto
                {
                    ScheduledOn = DateTime.Now,
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                };

            var regardingObject = ToEntityReference(parentEntityName, parentEntityId);
            if (regardingObject != null)
            {
                dto.RegardingObjects = new[] { regardingObject };
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Phonecall>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityName entityName, long? entityId)
        {
            if (!entityId.HasValue) return null;

            string name = null;
            switch (entityName)
            {
                case EntityName.Client:
                    name = _clientReadModel.GetClientName(entityId.Value);
                    break;
                case EntityName.Contact:
                    name = _clientReadModel.GetContactName(entityId.Value);
                    break;
                case EntityName.Deal:
                    name = _dealReadModel.GetDeal(entityId.Value).Name;
                    break;
                case EntityName.Firm:
                    name = _firmReadModel.GetFirmName(entityId.Value);
                    break;
            }

            return !string.IsNullOrEmpty(name)
                ? new EntityReference { Id = entityId, Name = name, EntityName = entityName }
                : null;
        }
    }
}