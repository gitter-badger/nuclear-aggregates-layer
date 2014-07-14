using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Metadata;

namespace DoubleGis.Erm.Qds.Operations.Listing
{
    public sealed class QdsListFirmService : ListEntityDtoServiceBase<Firm, FirmGridDoc>
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly FilterHelper<FirmGridDoc> _filterHelper;
        private readonly IListGenericEntityDtoService<Firm, ListFirmDto> _legacyService;

        public QdsListFirmService(FilterHelper<FirmGridDoc> filterHelper, ISecurityServiceUserIdentifier userIdentifierService, IUserContext userContext, IListGenericEntityDtoService<Firm, ListFirmDto> legacyService)
        {
            _filterHelper = filterHelper;
            _userIdentifierService = userIdentifierService;
            _userContext = userContext;
            _legacyService = legacyService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            if (!QdsDefaultFilterMetadata.ContainsFilter<FirmGridDoc>(querySettings.FilterName))
            {
                return _legacyService.List(querySettings.SearchListModel);
            }

            bool forMe;
            if (querySettings.TryGetExtendedProperty("ForMe", out forMe))
            {
                var userId = _userContext.Identity.Code.ToString();
                _filterHelper.AddFilter(x => x.Term(y => y.OwnerCode, userId));
            }

            bool forReserve;
            if (querySettings.TryGetExtendedProperty("ForReserve", out forReserve))
            {
                var reserveId = _userIdentifierService.GetReserveUserIdentity().Code.ToString();
                _filterHelper.AddFilter(x => x.Term(y => y.OwnerCode, reserveId));
            }

            return _filterHelper.Search(querySettings);
        }
    }
}