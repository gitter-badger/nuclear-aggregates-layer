using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderPositionService : ListEntityDtoServiceBase<OrderPosition, ListOrderPositionDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public ListOrderPositionService(
            IFinder finder, FilterHelper filterHelper)
        {
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IEnumerable<ListOrderPositionDto> List(QuerySettings querySettings, out int count)
        {
            var query = _finder.FindAll<OrderPosition>();

            return query
                .Select(x => new ListOrderPositionDto
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        DiscountPercent = x.DiscountPercent,
                        DiscountSum = x.DiscountSum,
                        OrderId = x.OrderId,
                        PayablePlan = x.PayablePlan,
                        PositionName = x.PricePosition.Position.Name,
                        PricePerUnit = x.PricePerUnit,
                        IsDeleted = x.IsDeleted,
                    })
                .QuerySettings(_filterHelper, querySettings, out count);
        }
    }
}