using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

// ReSharper disable once CheckNamespace
namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetLetterDtoService : GetDomainEntityDtoServiceBase<Letter>
    {
        private readonly ILetterReadModel _letterReadModel;
        private readonly IClientReadModel _clientReadModel;
        private readonly IActivityReferenceReader _activityReferenceReader;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ISecurityServiceUserIdentifier _userIdentifier;

        public GetLetterDtoService(IUserContext userContext,
                                 ILetterReadModel letterReadModel,
                                 IClientReadModel clientReadModel,
                                 IActivityReferenceReader activityReferenceReader,
                                 IDealReadModel dealReadModel,
                                 IFirmReadModel firmReadModel,
                                 ISecurityServiceUserIdentifier userIdentifier)
            : base(userContext)
        {
            _letterReadModel = letterReadModel;
            _clientReadModel = clientReadModel;
            _activityReferenceReader = activityReferenceReader;
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

        protected override IDomainEntityDto<Letter> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            var userInfo = UserContext.Identity as IUserInfo ?? UserInfo.Empty;
            var dto = new LetterDomainEntityDto
                {
                    ScheduledOn = DateTime.Now,
                    Priority = ActivityPriority.Average,
                    Status = ActivityStatus.InProgress,
                    SenderRef = new EntityReference(userInfo.Code, userInfo.DisplayName) { EntityName = EntityName.User }
                };

            EntityReference regardingObject = null;
            if (parentEntityName.CanBeRegardingObject())
            {
                dto.IsNeedLookupInitialization = true;
                regardingObject = ToEntityReference(parentEntityName, parentEntityId);
            }
            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
            {
                dto.RegardingObjects = _activityReferenceReader.GetRegardingObjects(parentEntityName, parentEntityId.Value);
            }

            if (regardingObject != null)
            {
                dto.RegardingObjects = new[] { regardingObject };
            }

            var recipient = parentEntityName.CanBeContacted() ? ToEntityReference(parentEntityName, parentEntityId) : null;
            if (recipient != null)
            {
                dto.IsNeedLookupInitialization = true;
                dto.RecipientRef = recipient;
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Letter>> references)
        {
            return references.Select(x => ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityReference<Letter> reference)
        {
            return reference != null ? ToEntityReference(reference.TargetEntityName, reference.TargetEntityId) : null;
        }

        private EntityReference ToEntityReference(EntityName entityName, long? entityId)
        {
            if (!entityId.HasValue)
            {
                return null;
            }

            string name;
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
                case EntityName.User:
                    name = (_userIdentifier.GetUserInfo(entityId) ?? UserInfo.Empty).DisplayName;
                    break;
                default:
                    return null;
            }

            return new EntityReference { Id = entityId, Name = name, EntityName = entityName };
        }
    }
}