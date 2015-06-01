using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListPricePositionService : ListEntityDtoServiceBase<PricePosition, ListPricePositionDto>
    {
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;
        private readonly IOrderReadModel _orderReadModel;

        public ListPricePositionService(IQuery query, FilterHelper filterHelper, IOrderReadModel orderReadModel)
        {
            _query = query;
            _filterHelper = filterHelper;
            _orderReadModel = orderReadModel;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<PricePosition>();

            var restrictByOrderSalesModelFilter = querySettings.CreateForExtendedProperty<PricePosition, long>(
                "orderId",
                orderId =>
                    {
                        var orderSalesModel = _orderReadModel.GetOrderSalesModel(orderId);

                        if (orderSalesModel == SalesModel.None)
                        {
                            return x => true;
                        }

                    return x => x.Position.SalesModel == orderSalesModel;
                });

            return query
                .Filter(_filterHelper, restrictByOrderSalesModelFilter)
                .Select(x => new ListPricePositionDto
                {
                    Id = x.Id,
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
                    SortingIndex = x.Position.SortingIndex.HasValue ? x.Position.SortingIndex.Value : int.MaxValue,
                })
                .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListPricePositionDto dto)
        {
            dto.PriceName = string.Format("{0} ({1})", dto.BeginDate.ToShortDateString(), dto.OrganizationUnitName);
        }
    }
}