using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
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
        private readonly IActivityReferenceReader _activityReferenceReader;

        public GetLetterDtoService(IUserContext userContext,
                                 ILetterReadModel letterReadModel,
                                 IActivityReferenceReader activityReferenceReader)
            : base(userContext)
        {
            _letterReadModel = letterReadModel;
            _activityReferenceReader = activityReferenceReader;
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

            if (parentEntityName.CanBeRegardingObject())
            {
                var regardingObject = _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId);                
                if (regardingObject.Id != null)
                {
                    dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(regardingObject);
                    dto.RecipientRef = _activityReferenceReader.FindClientContact(dto.RegardingObjects);
                }
            }
            else if (parentEntityName.IsActivity() && parentEntityId.HasValue)
            {
                dto.RegardingObjects = _activityReferenceReader.GetRegardingObjects(parentEntityName, parentEntityId.Value);
            }

            var recipient = parentEntityName.CanBeContacted() ? _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId) : null;
            if (recipient != null)
            {
                dto.RecipientRef = recipient;
                dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(recipient);
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Letter>> references)
        {
            return references.Select(x => _activityReferenceReader.ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }

        private EntityReference ToEntityReference(EntityReference<Letter> reference)
        {
            return reference != null ? _activityReferenceReader.ToEntityReference(reference.TargetEntityName, reference.TargetEntityId) : null;
        }
    }
}