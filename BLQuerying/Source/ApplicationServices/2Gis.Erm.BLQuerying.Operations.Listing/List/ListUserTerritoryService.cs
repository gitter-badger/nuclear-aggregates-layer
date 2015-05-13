using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListUserTerritoryService : ListEntityDtoServiceBase<UserTerritory, ListUserTerritoryDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListUserTerritoryService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<UserTerritory>();

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