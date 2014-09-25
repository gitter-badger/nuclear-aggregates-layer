﻿using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
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

            return query
                .Where(x => !x.IsDeleted)
                .Filter(_filterHelper,
                    myTerritoryFilter,
                    myBranchFilter,
                    selfAdsOrdersFilter)
                .Select(x => new ListFirmDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CreatedOn = x.CreatedOn,

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
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListFirmDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}