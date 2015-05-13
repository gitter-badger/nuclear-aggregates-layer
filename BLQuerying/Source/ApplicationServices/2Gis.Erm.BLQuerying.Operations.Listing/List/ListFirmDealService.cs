using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmDealService : ListEntityDtoServiceBase<FirmDeal, ListFirmDealDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;

        public ListFirmDealService(
            IQuery query,
            FilterHelper filterHelper,
            ISecurityServiceUserIdentifier userIdentifierService)
        {
            _query = query;
            _filterHelper = filterHelper;
            _userIdentifierService = userIdentifierService;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<FirmDeal>();

            return query
                .Filter(_filterHelper)
                .Select(x => new ListFirmDealDto
                                 {
                                     Id = x.Id,
                                     FirmId = x.FirmId,
                                     DealId = x.DealId,
                                     FirmName = x.Firm.Name,

                                     ClientId = x.Firm.Client != null ? x.Firm.Client.Id : (long?)null,
                                     ClientName = x.Firm.Client != null ? x.Firm.Client.Name : null,

                                     OwnerCode = x.Firm.OwnerCode,

                                     TerritoryId = x.Firm.Territory != null ? x.Firm.Territory.Id : (long?)null,
                                     TerritoryName = x.Firm.Territory != null ? x.Firm.Territory.Name : null,

                                     PromisingScore = x.Firm.PromisingScore,

                                     IsDeleted = x.IsDeleted,
                                     ClosedForAscertainment = x.Firm.ClosedForAscertainment,
                                     OwnerName = null,
                                 })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListFirmDealDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}