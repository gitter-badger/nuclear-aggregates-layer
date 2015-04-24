using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

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
                    ScheduledOn = phonecall.ScheduledOn,
                    Priority = phonecall.Priority,
                    Purpose = phonecall.Purpose,
                    Status = phonecall.Status,
                    RegardingObjects = AdaptReferences(regardingObjects),
                    RecipientRef = recipient != null ? ToEntityReference(recipient.TargetEntityTypeId, recipient.TargetEntityId) : null,

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
            var dto = new PhonecallDomainEntityDto
                {
                    ScheduledOn = DateTime.Now,
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                };

            var regardingObject = parentEntityName.CanBeRegardingObject() ? ToEntityReference(parentEntityName.Id, parentEntityId) : null;
            if (regardingObject != null)
            {
                dto.RegardingObjects = new[] { regardingObject };
            }

            var recipient = parentEntityName.CanBeContacted() ? ToEntityReference(parentEntityName.Id, parentEntityId) : null;
            if (recipient != null)
            {
                dto.RecipientRef = recipient;
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Phonecall>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityTypeId, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(int entityTypeId, long? entityId)
        {
            if (!entityId.HasValue)
            {
                return null;
            }

            string name;
            if (entityTypeId == EntityType.Instance.Client().Id)
            {
                name = _clientReadModel.GetClientName(entityId.Value);
            }
            else if (entityTypeId == EntityType.Instance.Contact().Id)
            {
                name = _clientReadModel.GetContactName(entityId.Value);
            }
            else if (entityTypeId == EntityType.Instance.Deal().Id)
            {
                name = _dealReadModel.GetDeal(entityId.Value).Name;
            }
            else if (entityTypeId == EntityType.Instance.Firm().Id)
            {
                name = _firmReadModel.GetFirmName(entityId.Value);
            }
            else
            {
                return null;
            }

            return new EntityReference { Id = entityId, Name = name, EntityTypeId = entityTypeId };
        }
    }
}