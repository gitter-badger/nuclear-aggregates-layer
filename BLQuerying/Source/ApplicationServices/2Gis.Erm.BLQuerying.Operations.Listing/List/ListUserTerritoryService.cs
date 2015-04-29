using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserTerritoryService : ListEntityDtoServiceBase<UserTerritory, ListUserTerritoryDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListUserTerritoryService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<UserTerritory>();

            var data = query
            .Select(x => new ListUserTerritoryDto
            {
                Id = x.Id,
                TerritoryId = x.TerritoryId,
                TerritoryName = x.TerritoryDto.Name,
                UserId = x.UserId,
                TerritoryIsActive = x.TerritoryDto != null,
                IsDeleted = x.IsDeleted,
            })
            .QuerySettings(_filterHelper, querySettings);

            return data;
        }
    }
}