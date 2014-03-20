using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdditionalFirmServiceService : ListEntityDtoServiceBase<AdditionalFirmService, ListAdditionalFirmServiceDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListAdditionalFirmServiceService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListAdditionalFirmServiceDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<AdditionalFirmService>();

            var dto = query
                .Select(x => new ListAdditionalFirmServiceDto
                {
                    Id = x.Id,
                    ServiceCode = x.ServiceCode,
                    Description = x.Description,
                    IsManaged = x.IsManaged,
                })
                .QuerySettings(_filterHelper, querySettings, out count);

            return dto;
        }
    }
}