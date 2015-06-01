using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderPositionAdvertisementService : ListEntityDtoServiceBase<OrderPositionAdvertisement, ListOrderPositionAdvertisementDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListOrderPositionAdvertisementService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<OrderPositionAdvertisement>();

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