﻿using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityService : ListEntityDtoServiceBase<Activity, ListActivityDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListActivityService(ISecurityServiceUserIdentifier userIdentifierService,
                                   ICompositeEntityDecorator compositeEntityDecorator,
                                   IUserContext userContext,
                                   IFinder finder,
                                   FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _compositeEntityDecorator = compositeEntityDecorator;
            _userContext = userContext;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            bool filterByParent;
            var parentEntityId = querySettings.ParentEntityId;
            var parentEntityName = querySettings.ParentEntityName;
            querySettings.TryGetExtendedProperty("filterByParents", out filterByParent);

            var appointmentDtos = ListAppointments(filterByParent, parentEntityName, parentEntityId);
            var letterDtos = ListLetters(filterByParent, parentEntityName, parentEntityId);
            var phonecalls = ListPhonecalls(filterByParent, parentEntityName, parentEntityId);
            var taskDtos = ListTasks(filterByParent, parentEntityName, parentEntityId);

            var activities = appointmentDtos.Concat(letterDtos).Concat(phonecalls).Concat(taskDtos);

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                activities = _filterHelper.ForSubordinates(activities);
            }

            return activities.QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListActivityDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }

        private IQueryable<ListActivityDto> ListAppointments(bool filterByParent, EntityName entityName, long? entityId)
        {
            Expression<Func<Appointment, bool>> filter = _ => true;
            if (filterByParent && entityId.HasValue)
            {
                switch (entityName)
                {
                    case EntityName.Client:
                    case EntityName.Deal:
                    case EntityName.Firm:
                        filter = FilterByReference<Appointment, AppointmentRegardingObject>(entityName, entityId.Value);
                        break;
                    case EntityName.Contact:
                        filter = FilterByReference<Appointment, AppointmentAttendee>(entityName, entityId.Value);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            var appointments = _compositeEntityDecorator.Find(Specs.Find.Active<Appointment>());

            return
                from appointment in appointments.Where(filter)
                select new ListActivityDto
                    {
                        ActivityTypeEnum = ActivityType.Appointment,
                        Id = appointment.Id,
                        OwnerCode = appointment.OwnerCode,
                        Header = appointment.Header,
                        ScheduledStart = appointment.ScheduledStart,
                        ScheduledEnd = appointment.ScheduledEnd,
                        ActualEnd = appointment.Status == ActivityStatus.Completed || appointment.Status == ActivityStatus.Canceled ? appointment.ModifiedOn : null,
                        StatusEnum = appointment.Status,
                        IsDeleted = appointment.IsDeleted,
                        IsActive = appointment.IsActive,
                        TaskType = TaskType.NotSet,
                        ActivityType = ActivityType.Appointment.ToStringLocalizedExpression(),
                        Priority = appointment.Priority.ToStringLocalizedExpression(),
                        Status = appointment.Status.ToStringLocalizedExpression(),
                    };
        }

        private IQueryable<ListActivityDto> ListLetters(bool filterByParent, EntityName entityName, long? entityId)
        {
            Expression<Func<Letter, bool>> filter = _ => true;
            if (filterByParent && entityId.HasValue)
            {
                switch (entityName)
                {
                    case EntityName.Client:
                    case EntityName.Deal:
                    case EntityName.Firm:
                        filter = FilterByReference<Letter, LetterRegardingObject>(entityName, entityId.Value);
                        break;
                    case EntityName.Contact:
                        filter = FilterByReference<Letter, LetterRecipient>(entityName, entityId.Value);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            var letters = _compositeEntityDecorator.Find(Specs.Find.Active<Letter>());

            return
                from letter in letters.Where(filter)
                select new ListActivityDto
                    {
                        ActivityTypeEnum = ActivityType.Letter,
                        Id = letter.Id,
                        OwnerCode = letter.OwnerCode,
                        Header = letter.Header,
                        ScheduledStart = letter.ScheduledOn,
                        ScheduledEnd = null,
                        ActualEnd = letter.Status == ActivityStatus.Completed || letter.Status == ActivityStatus.Canceled ? letter.ModifiedOn : null,
                        StatusEnum = letter.Status,
                        IsDeleted = letter.IsDeleted,
                        IsActive = letter.IsActive,
                        TaskType = TaskType.NotSet,
                        ActivityType = ActivityType.Letter.ToStringLocalizedExpression(),
                        Priority = letter.Priority.ToStringLocalizedExpression(),
                        Status = letter.Status.ToStringLocalizedExpression(),
                    };
        }

        private IQueryable<ListActivityDto> ListPhonecalls(bool filterByParent, EntityName entityName, long? entityId)
        {
            Expression<Func<Phonecall, bool>> filter = _ => true;
            if (filterByParent && entityId.HasValue)
            {
                switch (entityName)
                {
                    case EntityName.Client:
                    case EntityName.Deal:
                    case EntityName.Firm:
                        filter = FilterByReference<Phonecall, PhonecallRegardingObject>(entityName, entityId.Value);
                        break;
                    case EntityName.Contact:
                        filter = FilterByReference<Phonecall, PhonecallRecipient>(entityName, entityId.Value);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            var phonecalls = _compositeEntityDecorator.Find(Specs.Find.Active<Phonecall>());

            return
                from phonecall in phonecalls.Where(filter)
                select new ListActivityDto
                    {
                        ActivityTypeEnum = ActivityType.Phonecall,
                        Id = phonecall.Id,
                        OwnerCode = phonecall.OwnerCode,
                        Header = phonecall.Header,
                        ScheduledStart = phonecall.ScheduledOn,
                        ScheduledEnd = null,
                        ActualEnd = phonecall.Status == ActivityStatus.Completed || phonecall.Status == ActivityStatus.Canceled ? phonecall.ModifiedOn : null,
                        StatusEnum = phonecall.Status,
                        IsDeleted = phonecall.IsDeleted,
                        IsActive = phonecall.IsActive,
                        TaskType = TaskType.NotSet,
                        ActivityType = ActivityType.Phonecall.ToStringLocalizedExpression(),
                        Priority = phonecall.Priority.ToStringLocalizedExpression(),
                        Status = phonecall.Status.ToStringLocalizedExpression(),
                    };
        }

        private IQueryable<ListActivityDto> ListTasks(bool filterByParent, EntityName entityName, long? entityId)
        {
            Expression<Func<Task, bool>> filter = _ => true;
            if (filterByParent && entityId.HasValue)
            {
                switch (entityName)
                {
                    case EntityName.Client:
                    case EntityName.Deal:
                    case EntityName.Firm:
                        filter = FilterByReference<Task, TaskRegardingObject>(entityName, entityId.Value);
                        break;
                    case EntityName.Contact:
                        filter = _ => false;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            var tasks = _compositeEntityDecorator.Find(Specs.Find.Active<Task>());

            return
                from task in tasks.Where(filter)
                select new ListActivityDto
                    {
                        ActivityTypeEnum = ActivityType.Task,
                        Id = task.Id,
                        OwnerCode = task.OwnerCode,
                        Header = task.Header,
                        ScheduledStart = task.ScheduledOn,
                        ScheduledEnd = null,
                        ActualEnd = task.Status == ActivityStatus.Completed || task.Status == ActivityStatus.Canceled ? task.ModifiedOn : null,
                        StatusEnum = task.Status,
                        IsDeleted = task.IsDeleted,
                        IsActive = task.IsActive,
                        TaskType = task.TaskType,
                        ActivityType = ActivityType.Task.ToStringLocalizedExpression(),
                        Priority = task.Priority.ToStringLocalizedExpression(),
                        Status = task.Status.ToStringLocalizedExpression(),
                    };
        }

        private Expression<Func<TActivity,bool>> FilterByReference<TActivity, TEntityReference>(EntityName entityName, long entityId)
            where TActivity : class, IEntity, IEntityKey, IDeactivatableEntity
            where TEntityReference : EntityReference<TActivity>, IEntity
        {
            if (entityName == EntityName.Client)
            {
                var regardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<TEntityReference>(x => x.TargetEntityName == EntityName.Client));
                var regardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<TEntityReference>(x => x.TargetEntityName == EntityName.Firm));
                var regardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<TEntityReference>(x => x.TargetEntityName == EntityName.Deal));
                var firms = _finder.Find(Specs.Find.Active<Firm>());
                var deals = _finder.Find(Specs.Find.Active<Deal>());

                return activity => (from clientRef in regardingClients
                                    where clientRef.TargetEntityId == entityId
                                    select clientRef.SourceEntityId)
                                       .Union(from firmRef in regardingFirms
                                              join firm in firms on firmRef.TargetEntityId equals firm.Id
                                              where firm.ClientId == entityId
                                              select firmRef.SourceEntityId)
                                       .Union(from dealRef in regardingDeals
                                              join deal in deals on dealRef.TargetEntityId equals deal.Id
                                              where deal.ClientId == entityId
                                              select dealRef.SourceEntityId)
                                       .Contains(activity.Id);
            }

            if (entityName == EntityName.Deal || entityName == EntityName.Firm || entityName == EntityName.Contact)
            {
                var activities = _compositeEntityDecorator.Find(ActivitySpecs.Find.ByReferencedObject<TActivity, TEntityReference>(entityName, entityId));
                return activity => (from referencedEntity in activities select referencedEntity.SourceEntityId).Contains(activity.Id);
            }

            throw new NotSupportedException();
        }
    }
}