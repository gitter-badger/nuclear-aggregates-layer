using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<OrderPositionAdvertisement>();

            return query
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
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}