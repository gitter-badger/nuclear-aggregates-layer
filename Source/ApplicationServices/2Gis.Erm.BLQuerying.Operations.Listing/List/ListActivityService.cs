using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityService : ListEntityDtoServiceBase<Activity, ListActivityDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListActivityService(ISecurityServiceUserIdentifier userIdentifierService,
            ICompositeEntityDecorator compositeEntityDecorator,
            IFinder finder,
            IUserContext userContext,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _compositeEntityDecorator = compositeEntityDecorator;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        private IQueryable<ListActivityDto> ListAppointments()
        {
            var appointments = _compositeEntityDecorator.Find(Specs.Find.Active<Appointment>());
            var appointmentRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == EntityName.Client));
            var appointmentRegardingContacts = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == EntityName.Contact));
            var appointmentRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == EntityName.Firm));
            var appointmentRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Appointment>>(x => x.TargetEntityName == EntityName.Deal));

            return
                from appointment in appointments
                from regardingClient in appointmentRegardingClients.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from regardingContact in appointmentRegardingContacts.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from regardingFirm in appointmentRegardingFirms.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from regardingDeal in appointmentRegardingDeals.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                select new ListActivityDto
                {
                    ActivityTypeEnum = ActivityType.Appointment,
                    Id = appointment.Id,
                    OwnerCode = appointment.OwnerCode,
                    Header = appointment.Header,
                    ScheduledStart = appointment.ScheduledStart,
                    ScheduledEnd = appointment.ScheduledEnd,
                    ActualEnd = appointment.Status == ActivityStatus.Completed || appointment.Status == ActivityStatus.Canceled ? appointment.ActualEnd : null,
                    StatusEnum = appointment.Status,
                    IsDeleted = appointment.IsDeleted,
                    IsActive = appointment.IsActive,
                    TaskType = TaskType.NotSet,
                    ClientId = regardingClient.TargetEntityId,
                    ContactId = regardingContact.TargetEntityId,
                    FirmId = regardingFirm.TargetEntityId,
                    DealId = regardingDeal.TargetEntityId,
					ActivityType = ActivityType.Appointment.ToStringLocalizedExpression(),
                    Priority = ActivityPriority.Average.ToStringLocalizedExpression(),
                    Status = appointment.Status.ToStringLocalizedExpression(),
                };
        }

        private IQueryable<ListActivityDto> ListPhonecalls()
        {
            var phonecalls = _compositeEntityDecorator.Find(Specs.Find.Active<Phonecall>());
            var phonecallRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityName == EntityName.Client));
            var phonecallRegardingContacts = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityName == EntityName.Contact));
            var phonecallRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityName == EntityName.Firm));
            var phonecallRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Phonecall>>(x => x.TargetEntityName == EntityName.Deal));

            return
                from phonecall in phonecalls
                from regardingClient in phonecallRegardingClients.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from regardingContact in phonecallRegardingContacts.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from regardingFirm in phonecallRegardingFirms.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from regardingDeal in phonecallRegardingDeals.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                select new ListActivityDto
                {
                    ActivityTypeEnum = ActivityType.Phonecall,
                    Id = phonecall.Id,
                    OwnerCode = phonecall.OwnerCode,
                    Header = phonecall.Header,
                    ScheduledStart = phonecall.ScheduledStart,
                    ScheduledEnd = phonecall.ScheduledEnd,
                    ActualEnd = phonecall.Status == ActivityStatus.Completed || phonecall.Status == ActivityStatus.Canceled ? phonecall.ActualEnd : null,
                    StatusEnum = phonecall.Status,
                    IsDeleted = phonecall.IsDeleted,
                    IsActive = phonecall.IsActive,
                    TaskType = TaskType.NotSet,
                    ClientId = regardingClient.TargetEntityId,
                    ContactId = regardingContact.TargetEntityId,
                    FirmId = regardingFirm.TargetEntityId,
                    DealId = regardingDeal.TargetEntityId,
					ActivityType = ActivityType.Phonecall.ToStringLocalizedExpression(),
                    Priority = phonecall.Priority.ToStringLocalizedExpression(),
                    Status = phonecall.Status.ToStringLocalizedExpression(),
                };
        }

        private IQueryable<ListActivityDto> ListTasks()
        {
            var tasks = _compositeEntityDecorator.Find(Specs.Find.Active<Task>());
            var taskRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityName == EntityName.Client));
            var taskRegardingContacts = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityName == EntityName.Contact));
            var taskRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityName == EntityName.Firm));
            var taskRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<RegardingObject<Task>>(x => x.TargetEntityName == EntityName.Deal));

            return
                from task in tasks
                from regardingClient in taskRegardingClients.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from regardingContact in taskRegardingContacts.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from regardingFirm in taskRegardingFirms.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from regardingDeal in taskRegardingDeals.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                select new ListActivityDto
                {
                    ActivityTypeEnum = ActivityType.Task,
                    Id = task.Id,
                    OwnerCode = task.OwnerCode,
                    Header = task.Header,
                    ScheduledStart = task.ScheduledStart,
                    ScheduledEnd = task.ScheduledEnd,
                    ActualEnd = task.Status == ActivityStatus.Completed || task.Status == ActivityStatus.Canceled ? task.ActualEnd : null,
                    StatusEnum = task.Status,
                    IsDeleted = task.IsDeleted,
                    IsActive = task.IsActive,
                    TaskType = task.TaskType,
                    ClientId = regardingClient.TargetEntityId,
                    ContactId = regardingContact.TargetEntityId,
                    FirmId = regardingFirm.TargetEntityId,
                    DealId = regardingDeal.TargetEntityId,
					ActivityType = ActivityType.Task.ToStringLocalizedExpression(),
                    Priority = task.Priority.ToStringLocalizedExpression(),
                    Status = task.Status.ToStringLocalizedExpression(),
                };
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var appointmentDtos = ListAppointments();
            var phonecalls = ListPhonecalls();
            var taskDtos = ListTasks();

            var activities = appointmentDtos.Concat(phonecalls).Concat(taskDtos);

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                activities = _filterHelper.ForSubordinates(activities);
            }

            var forTodayFilter = CreateForExtendedProperty(activities, "ForToday", querySettings, forToday =>
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return
                    x =>
                    userDateTimeTodayUtc <= x.ScheduledStart && x.ScheduledStart < userDateTimeTomorrowUtc ||
                    userDateTimeTodayUtc <= x.ScheduledEnd && x.ScheduledEnd < userDateTimeTomorrowUtc;
            });

            var forMeFilter = CreateForExtendedProperty(activities, "ForMe", querySettings, forMe =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var expiredFilter = CreateForExtendedProperty(activities, "Expired", querySettings, expired =>
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);

                if (expired)
                {
                    return x => x.ScheduledEnd < userDateTimeTodayUtc;
                }

                return x => x.ScheduledEnd >= userDateTimeTodayUtc;
            });

            var result = activities
                .Filter(_filterHelper, forTodayFilter, forMeFilter, expiredFilter)
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    return x;
                });

            return result;
        }

        private static Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity>(IQueryable<TEntity> query, string key, QuerySettings querySettings, Func<bool, Expression<Func<TEntity, bool>>> action)
        {
            return querySettings.CreateForExtendedProperty(key, action);
        }
    }
}