﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDealService : ListEntityDtoServiceBase<Deal, ListDealDto>
    {
        private readonly IUserContext _userContext;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListDealService(
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IEnumerable<ListDealDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Deal>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var barterOrdersFilter = querySettings.CreateForExtendedProperty<Deal, bool>("WithBarterOrders", info =>
            {
                return x => x.Orders.Any(y => !y.IsDeleted && y.IsActive && (y.OrderType == (int)OrderType.AdsBarter || y.OrderType == (int)OrderType.ProductBarter || y.OrderType == (int)OrderType.ServiceBarter));
            });

            var myBranchFilter = querySettings.CreateForExtendedProperty<Deal, bool>("MyBranch", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.Client.Territory.OrganizationUnit.UserTerritoriesOrganizationUnits.Any(y => y.UserId == userId);
            });

            var myFilter = querySettings.CreateForExtendedProperty<Deal, bool>("ForMe", info =>
            {
                var userId = _userContext.Identity.Code;
                return x => x.OwnerCode == userId;
            });

            var filterExpression = querySettings.CreateForExtendedProperty<Deal, long>(
                "OrderId", 
                orderId =>
                {
                    var orderInfo = _finder.FindAll<Order>()
                            .Where(x => x.Id == orderId)
                            .Select(x => new
                                {
                                    x.DealId,
                                    DealClientId = (long?)x.Deal.ClientId,
                                    LegalPersonClientId = x.LegalPerson.ClientId,
                                    DestOrganizationUnitId = (long?)x.DestOrganizationUnitId
                                })
                            .Single();
                        if (orderInfo.DealId.HasValue)
                        {
                            return x => x.Id != orderInfo.DealId.Value && x.ClientId == orderInfo.DealClientId.Value;
                        }

                        if (orderInfo.DestOrganizationUnitId.HasValue)
                        {
                            if (orderInfo.LegalPersonClientId.HasValue)
                            {
                                return x => x.ClientId == orderInfo.LegalPersonClientId.Value;
                            }

                            return x => x.Client.Territory.OrganizationUnitId == orderInfo.DestOrganizationUnitId.Value &&
                                        !x.Client.IsDeleted &&
                                        x.Client.Territory.IsActive;
                        }

                        return null;
                    });

            return query
                .Filter(_filterHelper, filterExpression, barterOrdersFilter, myBranchFilter, myFilter)
                .Select(x => new ListDealDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ClientId = x.ClientId,
                    ClientName = x.Client.Name,
                    MainFirmId = x.MainFirmId,
                    MainFirmName = x.Firm.Name,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    OwnerCode = x.OwnerCode,
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}