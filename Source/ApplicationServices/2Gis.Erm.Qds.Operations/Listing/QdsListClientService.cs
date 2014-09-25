using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Docs;
using DoubleGis.Erm.Qds.Operations.Metadata;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class QdsListClientService : ListEntityDtoServiceBase<Client, ClientGridDoc>
    {
        private readonly FilterHelper _filterHelper;
        private readonly IQdsExtendedInfoFilterMetadata _extendedInfoFilterMetadata;
        private readonly IListGenericEntityDtoService<Client, ListClientDto> _legacyService;

        public QdsListClientService(FilterHelper filterHelper, IListGenericEntityDtoService<Client, ListClientDto> legacyService, IQdsExtendedInfoFilterMetadata extendedInfoFilterMetadata)
        {
            _filterHelper = filterHelper;
            _legacyService = legacyService;
            _extendedInfoFilterMetadata = extendedInfoFilterMetadata;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (!_extendedInfoFilterMetadata.ContainsExtendedInfo<ClientGridDoc>(querySettings.ExtendedInfoMap))
            {
                return _legacyService.List(querySettings.SearchListModel);
            }

            return _filterHelper.Search<ClientGridDoc>(querySettings);
        }
    }
}