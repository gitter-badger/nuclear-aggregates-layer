using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity;
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

        private readonly IActivityReferenceReader _activityReferenceReader;

        public GetPhonecallDtoService(IUserContext userContext,
                                      IPhonecallReadModel activityReadModel,
                                      IActivityReferenceReader activityReferenceReader)
            : base(userContext)
        {
            _activityReadModel = activityReadModel;
            _activityReferenceReader = activityReferenceReader;
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
                    RecipientRef = recipient != null ? _activityReferenceReader.ToEntityReference(recipient.TargetEntityName, recipient.TargetEntityId) : null,

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
                var attandees = _activityReferenceReader.GetAttendees(parentEntityName, parentEntityId.Value);
                var entityReferences = attandees as EntityReference[] ?? attandees.ToArray();
                if (entityReferences.Any() && entityReferences.Count() == 1)
                {
                    dto.RecipientRef = entityReferences.First();
                }
            }           

            var recipient = parentEntityName.CanBeContacted() ? _activityReferenceReader.ToEntityReference(parentEntityName, parentEntityId) : null;
            if (recipient != null)
            {
                dto.RecipientRef = recipient;
                dto.RegardingObjects = _activityReferenceReader.FindAutoCompleteReferences(recipient);
            }

            return dto;
        }

        private IEnumerable<EntityReference> AdaptReferences(IEnumerable<EntityReference<Phonecall>> references)
        {
            return references.Select(x => _activityReferenceReader.ToEntityReference(x.TargetEntityName, x.TargetEntityId)).Where(x => x != null).ToList();
        }
    }
}