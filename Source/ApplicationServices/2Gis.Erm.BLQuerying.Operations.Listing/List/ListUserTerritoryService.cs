using System.Collections.Generic;
using System.Linq;

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

        public ListUserTerritoryService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListUserTerritoryDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<UserTerritory>();

            var data = query
            .DefaultFilter(_filterHelper, querySettings)
            .Select(x => new ListUserTerritoryDto
            {
                Id = x.Id,
                TerritoryId = x.TerritoryId,
                TerritoryName = x.TerritoryDto.Name,
                IsDeleted = x.IsDeleted,
                UserId = x.UserId,
            })
            .QuerySettings(_filterHelper, querySettings, out count);

            return data;
        }
    }
}