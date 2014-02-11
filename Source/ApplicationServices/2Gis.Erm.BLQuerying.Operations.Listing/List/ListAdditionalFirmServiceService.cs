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
    public sealed class ListAdditionalFirmServiceService : ListEntityDtoServiceBase<AdditionalFirmService, ListAdditionalFirmServiceDto>
    {
        public ListAdditionalFirmServiceService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAdditionalFirmServiceDto> GetListData(IQueryable<AdditionalFirmService> query, QuerySettings querySettings, out int count)
        {
            var dto = query.Select(x => new ListAdditionalFirmServiceDto
            {
                Id = x.Id,
                ServiceCode = x.ServiceCode,
                Description = x.Description,
                IsManaged = x.IsManaged,
            })
            .ApplyQuerySettings(querySettings, out count).ToArray();

            return dto;
        }
    }
}