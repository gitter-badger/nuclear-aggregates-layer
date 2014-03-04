using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListTimeZoneService : ListEntityDtoServiceBase<TimeZone, ListTimeZoneDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListTimeZoneService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinder finder, FilterHelper filterHelper)
            : base(querySettingsProvider)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListTimeZoneDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<TimeZone>();

            return query
                .DefaultFilter(_filterHelper, querySettings)
                .Select(x => new ListTimeZoneDto
                    {
                        Id = x.Id,
                        TimeZoneId = x.TimeZoneId,
                        Description = x.Description,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}