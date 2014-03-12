using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderPositionAdvertisementService : ListEntityDtoServiceBase<OrderPositionAdvertisement, ListOrderPositionAdvertisementDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListOrderPositionAdvertisementService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListOrderPositionAdvertisementDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<OrderPositionAdvertisement>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListOrderPositionAdvertisementDto
                    {
                        Id = x.Id,
                        AdvertisementId = x.AdvertisementId,
                        CategoryId = x.CategoryId,
                        FirmAddressId = x.FirmAddressId,
                        OrderPositionId = x.OrderPositionId,
                        PositionId = x.PositionId,
                        ThemeId = x.ThemeId,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}