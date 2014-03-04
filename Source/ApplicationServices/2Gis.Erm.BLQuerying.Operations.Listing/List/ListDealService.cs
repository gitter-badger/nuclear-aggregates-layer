using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDealService : ListEntityDtoServiceBase<Deal, ListDealDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListDealService(
            IQuerySettingsProvider querySettingsProvider,
            IFinder finder,
            FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListDealDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Deal>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

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
                .Filter(_filterHelper, filterExpression)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(d => new ListDealDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    ClientId = d.ClientId,
                    ClientName = d.Client.Name,
                    MainFirmId = d.MainFirmId,
                    MainFirmName = d.Firm.Name
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}