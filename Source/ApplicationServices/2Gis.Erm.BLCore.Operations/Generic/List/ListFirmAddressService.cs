using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
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

        protected override IEnumerable<ListFirmAddressDto> GetListData(IQueryable<FirmAddress> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .Where(x => !x.Firm.IsDeleted)
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