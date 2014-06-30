using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmService : ListEntityDtoServiceBase<Firm, ListFirmDto>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListFirmService(
            ISecurityServiceUserIdentifier userIdentifierService,
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Firm>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var myTerritoryFilter = querySettings.CreateForExtendedProperty<Firm, bool>("MyTerritory", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Territory.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var myBranchFilter = querySettings.CreateForExtendedProperty<Firm, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var selfAdsOrdersFilter = querySettings.CreateForExtendedProperty<Firm, bool>("WithSelfAdsOrders", info =>
            {
                return x => x.Orders.Any(y => !y.IsDeleted && y.IsActive && y.OrderType == (int)OrderType.SelfAds);
            });

            var reserveFilter = querySettings.CreateForExtendedProperty<Firm, bool>("ForReserve", info =>
            {
                var reserveId = _userIdentifierService.GetReserveUserIdentity().Code;
                return x => x.OwnerCode == reserveId;
            });

            var myFilter = querySettings.CreateForExtendedProperty<Firm, bool>("ForMe", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var createdInCurrentMonthFilter = querySettings.CreateForExtendedProperty<Firm, bool>(
                "CreatedInCurrentMonth",
                createdInCurrentMonth =>
                    {
                        if (!createdInCurrentMonth)
                        {
                            return null;
                        }

                        var nextMonth = DateTime.Now.AddMonths(1);
                        nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                        var currentMonthLastDate = nextMonth.AddSeconds(-1);
                        var currentMonthFirstDate = new DateTime(currentMonthLastDate.Year, currentMonthLastDate.Month, 1);

                        return x => x.CreatedOn >= currentMonthFirstDate && x.CreatedOn <= currentMonthLastDate;
                    });

            var organizationUnitFilter = querySettings.CreateForExtendedProperty<Firm, long>(
                "organizationUnitId", organizationUnitId => x => x.OrganizationUnitId == organizationUnitId);

            var clientFilter = querySettings.CreateForExtendedProperty<Firm, long>(
                "clientId", clientId => x => x.ClientId == clientId);

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper,
                    clientFilter,
                    createdInCurrentMonthFilter,
                    organizationUnitFilter,
                    myTerritoryFilter,
                    myBranchFilter,
                    selfAdsOrdersFilter,
                    reserveFilter,
                    myFilter)
                .Select(x => new ListFirmDto
                    {
                        Id = x.Id,
                        Name = x.Name,

                        ClientId = x.Client != null ? x.Client.Id : (long?)null,
                        ClientName = x.Client != null ? x.Client.Name : null,

                        OwnerCode = x.OwnerCode,

                        TerritoryId = x.Territory != null ? x.Territory.Id : (long?)null,
                        TerritoryName = x.Territory != null ? x.Territory.Name : null,

                        PromisingScore = x.PromisingScore,
                        LastQualifyTime = x.LastQualifyTime,
                        LastDisqualifyTime = x.LastDisqualifyTime,
                        OrganizationUnitId = x.OrganizationUnit.Id,
                        OrganizationUnitName = x.OrganizationUnit.Name,

                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        ClosedForAscertainment = x.ClosedForAscertainment,
                        OwnerName = null,
                    })
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                {
                    x.OwnerName = _userIdentifierService.GetUserInfo(x.OwnerCode).DisplayName;
                    return x;
                });
        }
    }
}