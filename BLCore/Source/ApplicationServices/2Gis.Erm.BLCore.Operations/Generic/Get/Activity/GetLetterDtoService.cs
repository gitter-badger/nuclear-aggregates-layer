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

        private readonly ISecurityServiceUserIdentifier _userIdentifier;

        private readonly IClientReadModel _clientReadModel;
        private readonly IDealReadModel _dealReadModel;
        private readonly IFirmReadModel _firmReadModel;

        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRegardingObjects;
        private readonly Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>> _lookupsForRecipients;

        public GetLetterDtoService(IUserContext userContext,
                                   IAppointmentReadModel appointmentReadModel,
                                   IClientReadModel clientReadModel,
                                   IFirmReadModel firmReadModel,
                                   IDealReadModel dealReadModel,
                                   ILetterReadModel letterReadModel,
                                   ISecurityServiceUserIdentifier userIdentifier,
                                   IPhonecallReadModel phonecallReadModel,
                                   ITaskReadModel taskReadModel)
            : base(userContext)
        {
            _letterReadModel = letterReadModel;
            _userIdentifier = userIdentifier;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _dealReadModel = dealReadModel;

            var service = new ActivityReferenceReader(clientReadModel, dealReadModel, firmReadModel);

            _lookupsForRegardingObjects = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Task, entityId => taskReadModel.GetRegardingObjects(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, service.ResolveRegardingObjectsFromClient },
                { EntityName.Contact, service.ResolveRegardingObjectsFromContact },
                { EntityName.Deal, service.ResolveRegardingObjectsFromDeal },
                { EntityName.Firm, service.ResolveRegardingObjectsFromFirm },
            };

            _lookupsForRecipients = new Dictionary<EntityName, Func<long, IEnumerable<EntityReference>>>
            {
                { EntityName.Appointment, entityId => appointmentReadModel.GetAttendees(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Letter, entityId => letterReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Phonecall, entityId => phonecallReadModel.GetRecipient(entityId).ToEntityReferences().Select(EmbedEntityNameIfNeeded) },
                { EntityName.Client, service.ResolveContactsFromClient },
                { EntityName.Contact, service.ResolveContactsFromContact },
                { EntityName.Deal, service.ResolveContactsFromDeal },
                { EntityName.Firm, service.ResolveContactsFromFirm },
            };
        }

        protected override IDomainEntityDto<Letter> GetDto(long entityId)
        {
            var letter = _letterReadModel.GetLetter(entityId);
            if (letter == null)
            {
                throw new InvalidOperationException("The letter does not exist for the specified ID.");
            }

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
                    RegardingObjects = _lookupsForRegardingObjects.LookupElements(EntityName.Letter, entityId),
                    SenderRef = sender != null ? EmbedEntityNameIfNeeded(sender.ToEntityReference<Letter>()) : null,
                    RecipientRef = recipient != null ? EmbedEntityNameIfNeeded(recipient.ToEntityReference<Letter>()) : null,

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
            return new LetterDomainEntityDto
                       {
                           ScheduledOn = DateTime.Now,
                           Priority = ActivityPriority.Average,
                           Status = ActivityStatus.InProgress,
                           SenderRef = new EntityReference(userInfo.Code, userInfo.DisplayName) { EntityName = EntityName.User },

                           RegardingObjects = _lookupsForRegardingObjects.LookupElements(parentEntityName, parentEntityId),
                           RecipientRef = _lookupsForRecipients.LookupElements(parentEntityName, parentEntityId).FirstOrDefault(),
                       };
        }
        
        private EntityReference EmbedEntityNameIfNeeded(EntityReference reference)
        {
            if (reference.Id != null && reference.Name == null)
            {
                reference.Name = ReadEntityName(reference.EntityName, reference.Id.Value);
            }

            return reference;
        }

        private string ReadEntityName(EntityName entityName, long entityId)
        {
            switch (entityName)
            {
                case EntityName.Client:
                    return _clientReadModel.GetClientName(entityId);
                case EntityName.Contact:
                    return _clientReadModel.GetContactName(entityId);
                case EntityName.Deal:
                    return _dealReadModel.GetDeal(entityId).Name;
                case EntityName.Firm:
                    return _firmReadModel.GetFirmName(entityId);
                case EntityName.User:
                    return (_userIdentifier.GetUserInfo(entityId) ?? UserInfo.Empty).DisplayName;                    
                default:
                    throw new ArgumentOutOfRangeException("entityName");
            }
        }
    }
}