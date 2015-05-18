using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetLetterDtoService : GetDomainEntityDtoServiceBase<Letter>
    {
        private readonly ILetterReadModel _letterReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ISecurityServiceUserIdentifier _userIdentifier;

        public GetLetterDtoService(IUserContext userContext,
                                 ILetterReadModel letterReadModel,
                                 IClientReadModel clientReadModel,
                                 IDealReadModel dealReadModel,
                                 IFirmReadModel firmReadModel,
                                 ISecurityServiceUserIdentifier userIdentifier)
            : base(userContext)
        {
            _letterReadModel = letterReadModel;
            _clientReadModel = clientReadModel;
            _dealReadModel = dealReadModel;
            _firmReadModel = firmReadModel;
            _userIdentifier = userIdentifier;
        }

        protected override IDomainEntityDto<Letter> GetDto(long entityId)
        {
            var letter = _letterReadModel.GetLetter(entityId);
            if (letter == null)
            {
                throw new InvalidOperationException("The letter does not exist for the specified ID.");
            }

            var regardingObjects = _letterReadModel.GetRegardingObjects(entityId);
            var sender = _letterReadModel.GetSender(entityId);
            var recipient = _letterReadModel.GetRecipient(entityId);

            return new LetterDomainEntityDto
                {
                    Id = letter.Id,
                    Header = letter.Header,
                    Description = letter.Description,
                    ScheduledOn = letter.ScheduledOn,
                    Priority = letter.Priority,
                    Status = letter.Status,
                    RegardingObjects = AdaptReferences(regardingObjects),
                    SenderRef = ToEntityReference(sender),
                    RecipientRef = ToEntityReference(recipient),

                    OwnerRef = new EntityReference { Id = letter.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = letter.CreatedBy, Name = null },
                    CreatedOn = letter.CreatedOn,
                    ModifiedByRef = new EntityReference { Id = letter.ModifiedBy, Name = null },
                    ModifiedOn = letter.ModifiedOn,
                    IsActive = letter.IsActive,
                    IsDeleted = letter.IsDeleted,
                    Timestamp = letter.Timestamp,
                };
        }

        protected override IDomainEntityDto<Letter> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var userInfo = UserContext.Identity as IUserInfo ?? UserInfo.Empty;
            var dto = new LetterDomainEntityDto
                {
                    ScheduledOn = DateTime.Now,
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                    SenderRef = new EntityReference(userInfo.Code, userInfo.DisplayName) { EntityTypeId = EntityType.Instance.User().Id }
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

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Letter>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityTypeId, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityReference<Letter> reference)
        {
            return reference != null ? ToEntityReference(reference.TargetEntityTypeId, reference.TargetEntityId) : null;
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
            else if (entityTypeId == EntityType.Instance.User().Id)
            {
                name = (_userIdentifier.GetUserInfo(entityId) ?? UserInfo.Empty).DisplayName;
            }
            else
            {
                return null;
            }

            return new EntityReference { Id = entityId, Name = name, EntityTypeId = entityTypeId };
        }
    }
}