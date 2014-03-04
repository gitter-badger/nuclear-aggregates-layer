using System.Collections.Generic;
using System.Linq;

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

        public ListDeniedPositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListDeniedPositionDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<DeniedPosition>();

            var positionIdFilter = querySettings.CreateForExtendedProperty<DeniedPosition, long>(
                "PositionId",
                positionId => x => x.PositionId == positionId);

            var priceIdFilter = querySettings.CreateForExtendedProperty<DeniedPosition, long>(
                "PriceId",
                priceId => x => x.PriceId == priceId);

            return query
                .Filter(_filterHelper, positionIdFilter, priceIdFilter)
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListDeniedPositionDto
                {
                    Id = x.Id,
                    PositionDeniedId = x.PositionDeniedId,
                    PositionDeniedName = x.PositionDenied.Name,
                })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}