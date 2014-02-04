using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.EAV.PropertyIdentities;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListActivityService : ListEntityDtoServiceBase<ActivityInstance, ListActivityDto>
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListActivityService(IQuerySettingsProvider querySettingsProvider,
            IFinderBaseProvider finderBaseProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
            _userIdentifierService = userIdentifierService;
        }

        protected override IEnumerable<ListActivityDto> GetListData(IQueryable<ActivityInstance> query,
                                                                    QuerySettings querySettings,
                                                                    ListFilterManager filterManager,
                                                                    out int count)
        {
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

            var beginDay = DateTime.Today;
            var endDay = beginDay.AddDays(1).AddMilliseconds(-1);

            var forTodayFilter = CreateForTodayFilter(query2, filterManager, x => (x.ScheduledStart >= beginDay && x.ScheduledStart <= endDay) || (x.ScheduledEnd >= beginDay && x.ScheduledEnd <= endDay));

            var result = query2
                .ApplyFilter(forTodayFilter)
                .ApplyQuerySettings(querySettings, out count)
                .ToList()
                .Select(x => new ListActivityDto
                {
                    Id = x.Id,
                    Type = (ActivityType)x.ActivityType,
                    ActivityType = ((ActivityType)x.ActivityType).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                    OwnerCode = x.OwnerCode,
                    OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                    Header = x.Header,
                    ScheduledStart = x.ScheduledStart.Value,
                    ScheduledEnd = x.ScheduledEnd.Value,
                    Status = ((ActivityStatus)x.Status).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                    Priority = ((ActivityPriority)x.Priority).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo),
                    AfterSaleServiceType = x.AfterSaleServiceType.HasValue ? ((AfterSaleServiceType)(byte)x.AfterSaleServiceType).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo) : string.Empty,
                    ActualEnd = x.ActualEnd
                })
                .ToArray();

            return result;
        }

        private static Expression<Func<T, bool>> CreateForTodayFilter<T>(IQueryable<T> query, ListFilterManager filterManager, Expression<Func<T, bool>> expression)
        {
            return filterManager.CreateForExtendedProperty<T, bool>("ForToday",
                forToday =>
                {
                    if (!forToday)
                    {
                        return null;
                    }

                    return expression;
                });
        }
    }
}