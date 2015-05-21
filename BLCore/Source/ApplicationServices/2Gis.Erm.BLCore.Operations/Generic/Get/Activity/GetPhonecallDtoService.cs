﻿using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Security.API.UserContext;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetPhonecallDtoService : GetActivityDtoService<Phonecall>
    {
        private readonly IPhonecallReadModel _phonecallReadModel;

        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        public GetPhonecallDtoService(IUserContext userContext,
                                      IAppointmentReadModel appointmentReadModel,
                                      IClientReadModel clientReadModel,
                                      IFirmReadModel firmReadModel,
                                      IDealReadModel dealReadModel,
                                      ILetterReadModel letterReadModel,
                                      IPhonecallReadModel phonecallReadModel,
                                      ITaskReadModel taskReadModel)
            : base(userContext, appointmentReadModel, clientReadModel, firmReadModel, dealReadModel, letterReadModel, phonecallReadModel, taskReadModel)
        {
            _phonecallReadModel = phonecallReadModel;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;
        }

        protected override IDomainEntityDto<Phonecall> GetDto(long entityId)
        {
            var phonecall = _phonecallReadModel.GetPhonecall(entityId);
            if (phonecall == null)
            {
                throw new InvalidOperationException("The phonecall does not exist for the specified ID.");
            }

            var recipient = _phonecallReadModel.GetRecipient(entityId);

            return new PhonecallDomainEntityDto
                {
                    Id = phonecall.Id,
                    Header = phonecall.Header,
                    Description = phonecall.Description,
                    ScheduledOn = phonecall.ScheduledOn,
                    Priority = phonecall.Priority,
                    Purpose = phonecall.Purpose,
                    Status = phonecall.Status,
                    RegardingObjects = GetRegardingObjects(EntityType.Instance.Phonecall(), entityId),
                    RecipientRef = recipient != null ? EmbedEntityNameIfNeeded(recipient.ToEntityReference<Phonecall>()) : null,
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

        protected override IDomainEntityDto<Phonecall> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new PhonecallDomainEntityDto
                       {
                           ScheduledOn = DateTime.Now,
                           Priority = ActivityPriority.Average,
                           Status = ActivityStatus.InProgress,

                           RegardingObjects = GetRegardingObjects(parentEntityName, parentEntityId),
                           RecipientRef = GetAttandees(parentEntityName, parentEntityId).FirstOrDefault(),
                       };
        }

        private EntityReference EmbedEntityNameIfNeeded(EntityReference reference)
        {
            if (reference.Id != null && reference.Name == null)
            {
                reference.Name = ReadEntityName(reference.EntityTypeId, reference.Id.Value);
            }

            return reference;
        }

        private string ReadEntityName(long entityTypeId, long entityId)
        {
            if (entityTypeId == EntityType.Instance.Client().Id)
            {
                return _clientReadModel.GetClientName(entityId);
            }

            if (entityTypeId == EntityType.Instance.Contact().Id)
            {
                return _clientReadModel.GetContactName(entityId);
            }

            if (entityTypeId == EntityType.Instance.Deal().Id)
            {
                return _dealReadModel.GetDeal(entityId).Name;
            }

            if (entityTypeId == EntityType.Instance.Firm().Id)
            {
                return _firmReadModel.GetFirmName(entityId);
            }

            throw new ArgumentOutOfRangeException("entityTypeId");
        }
    }
}