using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
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

        public ListActivityInstanceService(IQuerySettingsProvider querySettingsProvider,
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            IUserContext userContext,
            FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _userContext = userContext;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListActivityInstanceDto> List(QuerySettings querySettings,
                                                                    out int count)
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

            var beginDay = DateTime.Today;
            var endDay = beginDay.AddDays(1).AddMilliseconds(-1);

            var forTodayFilter = CreateForTodayFilter(query2, querySettings, x => (x.ScheduledStart >= beginDay && x.ScheduledStart <= endDay) || (x.ScheduledEnd >= beginDay && x.ScheduledEnd <= endDay));

            var result = query2
                .Filter(_filterHelper, forTodayFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x => new ListActivityInstanceDto
                {
                    Id = x.Id,
                    Type = (ActivityType)x.ActivityType,
                    ActivityType = ((ActivityType)x.ActivityType).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    OwnerCode = x.OwnerCode,
                    OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName,
                    Header = x.Header,
                    ScheduledStart = x.ScheduledStart.Value,
                    ScheduledEnd = x.ScheduledEnd.Value,
                    Status = ((ActivityStatus)x.Status).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    Priority = ((ActivityPriority)x.Priority).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo),
                    AfterSaleServiceType = x.AfterSaleServiceType.HasValue ? ((AfterSaleServiceType)(byte)x.AfterSaleServiceType).ToStringLocalized(EnumResources.ResourceManager, _userContext.Profile.UserLocaleInfo.UserCultureInfo) : string.Empty,
                    ActualEnd = x.ActualEnd
                });

            return result;
        }

        private static Expression<Func<T, bool>> CreateForTodayFilter<T>(IQueryable<T> query, QuerySettings querySettings, Expression<Func<T, bool>> expression)
        {
            return querySettings.CreateForExtendedProperty<T, bool>("ForToday",
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