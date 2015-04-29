using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
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

        public ListTimeZoneService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.For<TimeZone>();

            return query
                .Select(x => new ListTimeZoneDto
                {
                    Id = x.Id,
                    TimeZoneId = x.TimeZoneId,
                })
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}
