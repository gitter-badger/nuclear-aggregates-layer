using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public class ListDeniedPositionService : ListEntityDtoServiceBase<DeniedPosition, ListDeniedPositionDto>
    {
        public ListDeniedPositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
        : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListDeniedPositionDto> GetListData(IQueryable<DeniedPosition> query, QuerySettings querySettings, out int count)
        {
            var positionIdFilter = querySettings.CreateForExtendedProperty<DeniedPosition, long>(
                "PositionId",
                positionId => x => x.PositionId == positionId);

            var priceIdFilter = querySettings.CreateForExtendedProperty<DeniedPosition, long>(
                "PriceId",
                priceId => x => x.PriceId == priceId);

            return query
                .ApplyFilter(positionIdFilter)
                .ApplyFilter(priceIdFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new ListDeniedPositionDto
                {
                    Id = x.Id,
                    PositionDeniedId = x.PositionDeniedId,
                    PositionDeniedName = x.PositionDenied.Name,
                });
        }
    }
}