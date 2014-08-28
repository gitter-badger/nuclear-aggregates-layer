using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityService : ListEntityDtoServiceBase<Activity, ListActivityInstanceDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly ICompositeEntityDecorator _compositeEntityDecorator;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListActivityService(ISecurityServiceUserIdentifier userIdentifierService,
            ICompositeEntityDecorator compositeEntityDecorator,
            IUserContext userContext,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _compositeEntityDecorator = compositeEntityDecorator;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            // FIXME {s.pomadin, 12.08.2014}: resolving of references is not processing
            // AfterSaleServiceTypeEnum = x.AfterSaleServiceType == null ? AfterSaleServiceType.None : (AfterSaleServiceType)(int)x.AfterSaleServiceType.Value,
            // ClientId = x.ClientId,
            // ContactId = x.ContactId,
            // DealId = x.DealId,
            // FirmId = x.FirmId,

            var activities =
                _compositeEntityDecorator.Find(Specs.Find.Active<Appointment>().Predicate).Select(x => new ListActivityInstanceDto
                    {
                        ActivityTypeEnum = ActivityType.Appointment,
                        Id = x.Id,
                        OwnerCode = x.OwnerCode,
                        Header = x.Header,
                        ScheduledStart = x.ScheduledStart,
                        ScheduledEnd = x.ScheduledEnd,
                        ActualEnd = x.ActualEnd,
                        PriorityEnum = x.Priority,
                        StatusEnum = x.Status,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive,
                        TaskType = TaskType.NotSet,
                    })
                .Concat(_compositeEntityDecorator.Find(Specs.Find.Active<Phonecall>().Predicate).Select(x => new ListActivityInstanceDto
                    {
                        ActivityTypeEnum = ActivityType.Phonecall,
                        Id = x.Id,
                        OwnerCode = x.OwnerCode,
                        Header = x.Header,
                        ScheduledStart = x.ScheduledStart,
                        ScheduledEnd = x.ScheduledEnd,
                        ActualEnd = x.ActualEnd,
                        PriorityEnum = x.Priority,
                        StatusEnum = x.Status,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive,
                        TaskType = TaskType.NotSet,
                    }))
                .Concat(_compositeEntityDecorator.Find(Specs.Find.Active<Task>().Predicate).Select(x => new ListActivityInstanceDto
                    {
                        ActivityTypeEnum = ActivityType.Task,
                        Id = x.Id,
                        OwnerCode = x.OwnerCode,
                        Header = x.Header,
                        ScheduledStart = x.ScheduledStart,
                        ScheduledEnd = x.ScheduledEnd,
                        ActualEnd = x.ActualEnd,
                        PriorityEnum = x.Priority,
                        StatusEnum = x.Status,
                        IsDeleted = x.IsDeleted,
                        IsActive = x.IsActive,
                        TaskType = x.TaskType,
                    }))
                ;

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
                    x.ActivityType = x.ActivityTypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    x.Status = x.StatusEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.Priority = x.PriorityEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                    x.AfterSaleServiceType = x.AfterSaleServiceTypeEnum.ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo);

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