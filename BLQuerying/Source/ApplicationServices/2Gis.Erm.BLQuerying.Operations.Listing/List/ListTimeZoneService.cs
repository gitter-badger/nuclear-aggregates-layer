using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListTimeZoneService : ListEntityDtoServiceBase<TimeZone, ListTimeZoneDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListTimeZoneService(IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<TimeZone>();

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
