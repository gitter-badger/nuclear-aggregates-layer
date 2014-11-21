using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Metadata;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class QdsListOrderService : ListEntityDtoServiceBase<Order, OrderGridDoc>
    {
        private readonly FilterHelper _filterHelper;
        private readonly IQdsExtendedInfoFilterMetadata _extendedInfoFilterMetadata;
        private readonly IListGenericEntityDtoService<Order, ListOrderDto> _legacyService;

        public QdsListOrderService(FilterHelper filterHelper, IListGenericEntityDtoService<Order, ListOrderDto> legacyService, IQdsExtendedInfoFilterMetadata extendedInfoFilterMetadata)
        {
            _filterHelper = filterHelper;
            _legacyService = legacyService;
            _extendedInfoFilterMetadata = extendedInfoFilterMetadata;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (!_extendedInfoFilterMetadata.ContainsExtendedInfo<OrderGridDoc>(querySettings.ExtendedInfoMap))
            {
                return _legacyService.List(querySettings.SearchListModel);
            }

            return _filterHelper.Search<OrderGridDoc>(querySettings);
        }
    }
}