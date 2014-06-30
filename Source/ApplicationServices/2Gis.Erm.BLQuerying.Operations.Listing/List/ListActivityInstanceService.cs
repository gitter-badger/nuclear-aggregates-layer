using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityInstanceService : ListEntityDtoServiceBase<ActivityInstance, ListActivityInstanceDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly IUserContext _userContext;
        private readonly FilterHelper _filterHelper;

        public ListActivityInstanceService(ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext,
            FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<ActivityInstance>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var query2 = query.Select(x => new
            {
                x.Id,
                x.ClientId,
                x.ContactId,
                x.FirmId,
                x.CreatedBy,
                x.CreatedOn,
                x.ModifiedBy,
                x.ModifiedOn,
                x.IsActive,
                x.IsDeleted,
                x.OwnerCode,
                ActivityType = x.Type,
                DealId = x.ActivityPropertyInstances.Where(y => y.PropertyId == DealIdIdentity.Instance.Id).Select(y => y.NumericValue).FirstOrDefault(),
                Description = x.ActivityPropertyInstances.Where(y => y.PropertyId == DescriptionIdentity.Instance.Id).Select(y => y.TextValue).FirstOrDefault(),
                Header = x.ActivityPropertyInstances.Where(y => y.PropertyId == HeaderIdentity.Instance.Id).Select(y => y.TextValue).FirstOrDefault(),
                ScheduledStart = x.ActivityPropertyInstances.Where(y => y.PropertyId == ScheduledStartIdentity.Instance.Id).Select(y => y.DateTimeValue).FirstOrDefault(),
                ScheduledEnd = x.ActivityPropertyInstances.Where(y => y.PropertyId == ScheduledEndIdentity.Instance.Id).Select(y => y.DateTimeValue).FirstOrDefault(),
                Status = x.ActivityPropertyInstances.Where(y => y.PropertyId == StatusIdentity.Instance.Id).Select(y => y.NumericValue).FirstOrDefault(),
                Priority = x.ActivityPropertyInstances.Where(y => y.PropertyId == PriorityIdentity.Instance.Id).Select(y => y.NumericValue).FirstOrDefault(),
                AfterSaleServiceType = x.ActivityPropertyInstances.Where(y => y.PropertyId == AfterSaleServiceTypeIdentity.Instance.Id).Select(y => y.NumericValue).FirstOrDefault(),
                TaskType = x.ActivityPropertyInstances.Where(y => y.PropertyId == TaskTypeIdentity.Instance.Id).Select(y => y.NumericValue).FirstOrDefault(),
                ActualEnd = x.ActivityPropertyInstances.Where(y => y.PropertyId == ActualEndIdentity.Instance.Id).Select(y => y.DateTimeValue).FirstOrDefault()
            });


            var forTodayFilter = CreateForExtendedProperty(query2, "ForToday", querySettings, forToday =>
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTomorrowUtc = userDateTimeTodayUtc.AddDays(1);

                return
                    x =>
                    userDateTimeTodayUtc <= x.ScheduledStart && x.ScheduledStart < userDateTimeTomorrowUtc ||
                    userDateTimeTodayUtc <= x.ScheduledEnd && x.ScheduledEnd < userDateTimeTomorrowUtc;
            });

            var forMeFilter = CreateForExtendedProperty(query2, "ForMe", querySettings, forMe =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var expiredFilter = CreateForExtendedProperty(query2, "Expired", querySettings, expired =>
            {
                var userDateTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);
                var userDateTimeTodayUtc = TimeZoneInfo.ConvertTimeToUtc(userDateTimeNow.Date, _userContext.Profile.UserLocaleInfo.UserTimeZoneInfo);

                if (expired)
                {
                    return x => x.ScheduledEnd < userDateTimeTodayUtc;
                }

                return x => x.ScheduledEnd >= userDateTimeTodayUtc;
            });

            var result = query2
                .Filter(_filterHelper, forTodayFilter, forMeFilter, expiredFilter)
                .Select(x => new ListActivityInstanceDto
                {
                    Id = x.Id,
                    ActivityTypeEnum = (ActivityType)x.ActivityType,
                    OwnerCode = x.OwnerCode,
                    Header = x.Header,
                    ScheduledStart = x.ScheduledStart.Value,
                    ScheduledEnd = x.ScheduledEnd.Value,
                    StatusEnum = (ActivityStatus)(int)x.Status.Value,
                    PriorityEnum = (ActivityPriority)(int)x.Priority.Value,
                    AfterSaleServiceTypeEnum = x.AfterSaleServiceType == null ? AfterSaleServiceType.None : (AfterSaleServiceType)(int)x.AfterSaleServiceType.Value,
                    ActualEnd = x.ActualEnd,
                    ClientId = x.ClientId,
                    ContactId = x.ContactId,
                    DealId = (long?)x.DealId,
                    FirmId = x.FirmId,
                    IsDeleted = x.IsDeleted,
                    IsActive = x.IsActive,
                    TaskType = x.TaskType == null ? ActivityTaskType.NotSet : (ActivityTaskType)(int)x.TaskType.Value,
                    OwnerName = null,
                    ActivityType = null,
                    AfterSaleServiceType = null,
                    Priority = null,
                    Status = null,
                })
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