using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPricePositionService : ListEntityDtoServiceBase<PricePosition, ListPricePositionDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListPricePositionService(IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListPricePositionDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<PricePosition>();

            return query
                .Select(x => new ListPricePositionDto
                {
                    Id=  x.Id,
                    PriceId = x.PriceId,
                    PositionName = x.Position.Name,
                    Cost = x.Cost,
                    PlatformId = x.Position.PlatformId,
                    OrganizationUnitName = x.Price.OrganizationUnit.Name,
                    BeginDate = x.Price.BeginDate,
                    PositionId = x.PositionId,
                    IsActive = x.IsActive,
                    IsDeleted = x.IsDeleted,
                    PriceName = null,
                })
                .QuerySettings(_filterHelper, querySettings, out count)
                .Select(x =>
                {
                    x.PriceName = string.Format("{0} ({1})", x.BeginDate.ToShortDateString(), x.OrganizationUnitName);
                    return x;
                });
        }
    }
}