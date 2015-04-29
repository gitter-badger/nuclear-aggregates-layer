using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListDeniedPositionService : ListEntityDtoServiceBase<DeniedPosition, ListDeniedPositionDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListDeniedPositionService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<DeniedPosition>();

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