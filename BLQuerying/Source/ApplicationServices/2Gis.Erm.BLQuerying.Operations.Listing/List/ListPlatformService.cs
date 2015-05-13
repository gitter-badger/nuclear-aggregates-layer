using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;

using NuClear.Storage;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPlatformService : ListEntityDtoServiceBase<Platform.Model.Entities.Erm.Platform, ListPlatformDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListPlatformService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Platform.Model.Entities.Erm.Platform>();

            return query
                .Select(x => new ListPlatformDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}