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
    public class ListFirmAddressService : ListEntityDtoServiceBase<FirmAddress, ListFirmAddressDto>
    {
        public ListFirmAddressService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListFirmAddressDto> GetListData(IQueryable<FirmAddress> query, QuerySettings querySettings, out int count)
        {
            var firmFilter = querySettings.CreateForExtendedProperty<FirmAddress, long>(
                "FirmId", firmId => x => x.FirmId == firmId);

            return query
                .Where(x => !x.Firm.IsDeleted)
                .ApplyFilter(firmFilter)
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new ListFirmAddressDto
                {
                    Id = x.Id,
                    FirmId = x.FirmId,
                    FirmName = x.Firm.Name,
                    Address = x.Address + ((x.ReferencePoint == null) ? string.Empty : " — " + x.ReferencePoint),
                });
        }
    }
}