using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDeniedPositionService : ListEntityDtoServiceBase<DeniedPosition, ListDeniedPositionDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListDeniedPositionService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<DeniedPosition>();

            return query
                .Select(x => new ListDeniedPositionDto
                {
                    Id = x.Id,
                    PositionDeniedId = x.PositionDeniedId,
                    PositionDeniedName = x.PositionDenied.Name,
                    PositionId = x.PositionId,
                    PriceId = x.PriceId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}