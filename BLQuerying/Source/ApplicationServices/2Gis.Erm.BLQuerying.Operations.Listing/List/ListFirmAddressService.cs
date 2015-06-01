using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListFirmAddressService : ListEntityDtoServiceBase<FirmAddress, ListFirmAddressDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListFirmAddressService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<FirmAddress>();

            return query
                .Where(x => !x.Firm.IsDeleted)
                .Select(x => new ListFirmAddressDto
                {
                    Id = x.Id,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.Name,
                    SortingPosition = x.SortingPosition,
                    Address = x.Address + ((x.ReferencePoint == null) ? string.Empty : " — " + x.ReferencePoint),
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    ClosedForAscertainment = x.ClosedForAscertainment,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}