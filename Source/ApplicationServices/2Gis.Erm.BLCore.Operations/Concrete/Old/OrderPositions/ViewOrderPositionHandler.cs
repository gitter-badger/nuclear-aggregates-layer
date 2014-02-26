using DoubleGis.Erm.BLCore.Aggregates.Firms;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class ViewOrderPositionHandler : RequestHandler<ViewOrderPositionRequest, ViewOrderPositionResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly IFirmRepository _firmRepository;

        public ViewOrderPositionHandler(IOrderReadModel orderReadModel, IPriceReadModel priceReadModel, IFirmRepository firmRepository)
        {
            _orderReadModel = orderReadModel;
            _priceReadModel = priceReadModel;
            _firmRepository = firmRepository;
        }

        protected override ViewOrderPositionResponse Handle(ViewOrderPositionRequest request)
        {
            var positionInfo = _orderReadModel.GetOrderPositionDetailedInfo(request.OrderPositionId, request.OrderId, request.PricePositionId, request.IncludeHidden);

            var categoryRate = _priceReadModel.GetCategoryRate(request.PricePositionId, _firmRepository.GetOrderFirmId(request.OrderId), request.CategoryId);

            var priceCalulations = _orderReadModel.CalculatePricePerUnit(request.OrderId, categoryRate, positionInfo.PricePositionCost);

            return new ViewOrderPositionResponse
            {
                OrderReleaseCountFact = positionInfo.ReleaseCountFact,
                OrderReleaseCountPlan = positionInfo.ReleaseCountPlan,
                PlatformName = positionInfo.Platform,
                PricePositionAmount = positionInfo.Amount,
                AmountSpecificationMode = positionInfo.AmountSpecificationMode,
                PricePositionCost = positionInfo.PricePositionCost,
                IsPositionComposite = positionInfo.IsComposite,
                IsBudget = positionInfo.IsBudget,
                LinkingObjectsSchema = positionInfo.LinkingObjectsSchema,
                PricePerUnit = priceCalulations.PricePerUnit,
                VatRatio = priceCalulations.VatRatio,
            };
        }
    }
}
