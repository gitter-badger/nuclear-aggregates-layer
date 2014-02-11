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
    public class ListPricePositionService : ListEntityDtoServiceBase<PricePosition, ListPricePositionDto>
    {
        public ListPricePositionService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListPricePositionDto> GetListData(IQueryable<PricePosition> query,
                                                                         QuerySettings querySettings,
                                                                         out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.PriceId,
                        PositionName = x.Position.Name,
                        x.Cost,
                        x.Position.PlatformId,
                        OrganizationUnitName = x.Price.OrganizationUnit.Name,
                        x.Price.BeginDate,
                        x.PositionId
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListPricePositionDto
                            {
                                Id = x.Id,
                                PriceId = x.PriceId,
                                PositionName = x.PositionName,
                                Cost = x.Cost,
                                PlatformId = x.PlatformId,
                                PriceName = string.Format("{0} ({1})", x.BeginDate.ToShortDateString(), x.OrganizationUnitName),
                                PositionId = x.PositionId
                            });
        }
    }
}