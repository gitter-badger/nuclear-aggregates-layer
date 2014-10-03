using System;
using System.Linq;
using System.Linq.Expressions;

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

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityService : ListEntityDtoServiceBase<Activity, ListActivityDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListActivityService(ISecurityServiceUserIdentifier userIdentifierService,
                                   ICompositeEntityDecorator compositeEntityDecorator,
                                   IFinder finder,
                                   FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _compositeEntityDecorator = compositeEntityDecorator;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        private IQueryable<ListActivityDto> ListAppointments()
        {
            var appointments = _compositeEntityDecorator.Find(Specs.Find.Active<Appointment>());
            var appointmentRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == EntityName.Client));
            var appointmentRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == EntityName.Firm));
            var appointmentRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<AppointmentRegardingObject>(x => x.TargetEntityName == EntityName.Deal));
            var firms = _finder.Find(Specs.Find.Active<Firm>());
            var deals = _finder.Find(Specs.Find.Active<Deal>());

            return
                from appointment in appointments
                from regardingClient in appointmentRegardingClients.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from regardingFirm in appointmentRegardingFirms.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from regardingDeal in appointmentRegardingDeals.Where(x => x.SourceEntityId == appointment.Id).DefaultIfEmpty()
                from firm in firms.Where(x => x.Id == regardingFirm.TargetEntityId).DefaultIfEmpty()
                from deal in deals.Where(x => x.Id == regardingDeal.TargetEntityId).DefaultIfEmpty()
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
                               ClientId = regardingClient.TargetEntityId,
                               FirmId = regardingFirm.TargetEntityId,
                               FirmClientId = firm.ClientId,
                               DealId = regardingDeal.TargetEntityId,
                               DealClientId = deal.ClientId,
                               ActivityType = ActivityType.Appointment.ToStringLocalizedExpression(),
                               Priority = ActivityPriority.Average.ToStringLocalizedExpression(),
                               Status = appointment.Status.ToStringLocalizedExpression(),
                           };
        }

        private IQueryable<ListActivityDto> ListLetters()
        {
            var letters = _compositeEntityDecorator.Find(Specs.Find.Active<Letter>());
            var letterRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<LetterRegardingObject>(x => x.TargetEntityName == EntityName.Client));
            var letterRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<LetterRegardingObject>(x => x.TargetEntityName == EntityName.Firm));
            var letterRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<LetterRegardingObject>(x => x.TargetEntityName == EntityName.Deal));
            var firms = _finder.Find(Specs.Find.Active<Firm>());
            var deals = _finder.Find(Specs.Find.Active<Deal>());

            return
                from letter in letters
                from regardingClient in letterRegardingClients.Where(x => x.SourceEntityId == letter.Id).DefaultIfEmpty()
                from regardingFirm in letterRegardingFirms.Where(x => x.SourceEntityId == letter.Id).DefaultIfEmpty()
                from regardingDeal in letterRegardingDeals.Where(x => x.SourceEntityId == letter.Id).DefaultIfEmpty()
                from firm in firms.Where(x => x.Id == regardingFirm.TargetEntityId).DefaultIfEmpty()
                from deal in deals.Where(x => x.Id == regardingDeal.TargetEntityId).DefaultIfEmpty()
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
                               ClientId = regardingClient.TargetEntityId,
                               FirmId = regardingFirm.TargetEntityId,
                               FirmClientId = firm.ClientId,
                               DealId = regardingDeal.TargetEntityId,
                               DealClientId = deal.ClientId,
                               ActivityType = ActivityType.Letter.ToStringLocalizedExpression(),
                               Priority = letter.Priority.ToStringLocalizedExpression(),
                               Status = letter.Status.ToStringLocalizedExpression(),
                           };
        }

        private IQueryable<ListActivityDto> ListPhonecalls()
        {
            var phonecalls = _compositeEntityDecorator.Find(Specs.Find.Active<Phonecall>());
            var phonecallRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == EntityName.Client));
            var phonecallRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == EntityName.Firm));
            var phonecallRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<PhonecallRegardingObject>(x => x.TargetEntityName == EntityName.Deal));
            var firms = _finder.Find(Specs.Find.Active<Firm>());
            var deals = _finder.Find(Specs.Find.Active<Deal>());

            return
                from phonecall in phonecalls
                from regardingClient in phonecallRegardingClients.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from regardingFirm in phonecallRegardingFirms.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from regardingDeal in phonecallRegardingDeals.Where(x => x.SourceEntityId == phonecall.Id).DefaultIfEmpty()
                from firm in firms.Where(x => x.Id == regardingFirm.TargetEntityId).DefaultIfEmpty()
                from deal in deals.Where(x => x.Id == regardingDeal.TargetEntityId).DefaultIfEmpty()
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
                               ClientId = regardingClient.TargetEntityId,
                               FirmId = regardingFirm.TargetEntityId,
                               FirmClientId = firm.ClientId,
                               DealId = regardingDeal.TargetEntityId,
                               DealClientId = deal.ClientId,
                               ActivityType = ActivityType.Phonecall.ToStringLocalizedExpression(),
                               Priority = phonecall.Priority.ToStringLocalizedExpression(),
                               Status = phonecall.Status.ToStringLocalizedExpression(),
                           };
        }

        private IQueryable<ListActivityDto> ListTasks()
        {
            var tasks = _compositeEntityDecorator.Find(Specs.Find.Active<Task>());
            var taskRegardingClients = _compositeEntityDecorator.Find(Specs.Find.Custom<TaskRegardingObject>(x => x.TargetEntityName == EntityName.Client));
            var taskRegardingFirms = _compositeEntityDecorator.Find(Specs.Find.Custom<TaskRegardingObject>(x => x.TargetEntityName == EntityName.Firm));
            var taskRegardingDeals = _compositeEntityDecorator.Find(Specs.Find.Custom<TaskRegardingObject>(x => x.TargetEntityName == EntityName.Deal));
            var firms = _finder.Find(Specs.Find.Active<Firm>());
            var deals = _finder.Find(Specs.Find.Active<Deal>());

            return
                from task in tasks
                from regardingClient in taskRegardingClients.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from regardingFirm in taskRegardingFirms.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from regardingDeal in taskRegardingDeals.Where(x => x.SourceEntityId == task.Id).DefaultIfEmpty()
                from firm in firms.Where(x => x.Id == regardingFirm.TargetEntityId).DefaultIfEmpty()
                from deal in deals.Where(x => x.Id == regardingDeal.TargetEntityId).DefaultIfEmpty()
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
                               ClientId = regardingClient.TargetEntityId,
                               FirmId = regardingFirm.TargetEntityId,
                               FirmClientId = firm.ClientId,
                               DealId = regardingDeal.TargetEntityId,
                               DealClientId = deal.ClientId,
                               ActivityType = ActivityType.Task.ToStringLocalizedExpression(),
                               Priority = task.Priority.ToStringLocalizedExpression(),
                               Status = task.Status.ToStringLocalizedExpression(),
                           };
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var appointmentDtos = ListAppointments();
            var letterDtos = ListLetters();
            var phonecalls = ListPhonecalls();
            var taskDtos = ListTasks();

            var activities = appointmentDtos.Concat(letterDtos).Concat(phonecalls).Concat(taskDtos);

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                activities = _filterHelper.ForSubordinates(activities);
            }

            var byParentFilter = CreateForExtendedProperty(activities,
                                                           "filterByParents",
                                                           querySettings,
                                                           byParent =>
                                                           {
                                                               var parentEntityId = querySettings.ParentEntityId;
                                                               var parentEntityName = querySettings.ParentEntityName;
                                                               return x =>
                                                                      (parentEntityName == EntityName.Client &&
                                                                       (x.ClientId == parentEntityId || x.FirmClientId == parentEntityId || x.DealClientId == parentEntityId))
                                                                      || (parentEntityName == EntityName.Firm && x.FirmId == parentEntityId)
                                                                      || (parentEntityName == EntityName.Deal && x.DealId == parentEntityId);
                                                           });

            var result = activities
                .Filter(_filterHelper, byParentFilter)
                .QuerySettings(_filterHelper, querySettings);
            return result;
        }

        protected override void Transform(ListActivityDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }

        private static Expression<Func<TEntity, bool>> CreateForExtendedProperty<TEntity>(IQueryable<TEntity> query, string key, QuerySettings querySettings, Func<bool, Expression<Func<TEntity, bool>>> action)
        {
            return querySettings.CreateForExtendedProperty(key, action);
        }
    }
}