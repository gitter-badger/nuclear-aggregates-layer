using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPlatformService : ListEntityDtoServiceBase<Platform.Model.Entities.Erm.Platform, ListPlatformDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPlatformService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPlatformDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<Platform.Model.Entities.Erm.Platform>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListPlatformDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}