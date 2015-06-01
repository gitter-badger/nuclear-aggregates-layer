using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListOrderPositionService : ListEntityDtoServiceBase<OrderPosition, ListOrderPositionDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListOrderPositionService(
            IQuery query, FilterHelper filterHelper)
        {
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<OrderPosition>();

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
                .QuerySettings(_filterHelper, querySettings);
        }
    }
}