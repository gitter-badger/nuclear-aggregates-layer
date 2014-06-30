using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Infrastructure;
using DoubleGis.Erm.Qds.Operations.Metadata;

namespace DoubleGis.Erm.Qds.Operations
{
    public sealed class QdsListOrderService : ListEntityDtoServiceBase<Order, OrderGridDoc>
    {
        private readonly FilterHelper<OrderGridDoc> _filterHelper;
        private readonly IListGenericEntityDtoService<Order, ListOrderDto> _legacyService;

        public QdsListOrderService(FilterHelper<OrderGridDoc> filterHelper, IListGenericEntityDtoService<Order, ListOrderDto> legacyService)
        {
            _filterHelper = filterHelper;
            _legacyService = legacyService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (!QdsDefaultFilterMetadata.ContainsFilter<OrderGridDoc>(querySettings.FilterName))
            {
                return _legacyService.List(querySettings.SearchListModel);
            }

            return _filterHelper.Search(querySettings);
        }
    }
}