using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Special.CostCalculation
{
    public class OrderCostCalculationOperationService : ICalculateOrderCostService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ICalculateOrderPositionCostService _calculateOrderPositionCostService;

        public OrderCostCalculationOperationService(IOrderRepository orderRepository,
                                                    IOperationScopeFactory scopeFactory,
                                                    ICalculateOrderPositionCostService calculateOrderPositionCostService)
        {
            _orderRepository = orderRepository;
            _scopeFactory = scopeFactory;
            _calculateOrderPositionCostService = calculateOrderPositionCostService;
        }

        public CalculationResult CalculateOrderProlongation(long orderId)
        {
            var orderInfo = _orderRepository.GetOrderForProlongationInfo(orderId);

            if (!orderInfo.Positions.Any())
            {
                throw new BusinessLogicException(BLResources.OrderToProlongateDoesntHaveAnyPositions);
            }

            var positionInfos = orderInfo.Positions.Select(x => new CalcPositionWithDiscountInfo
                {
                    PositionInfo = new CalcPositionInfo
                        {
                            PositionId = x.PositionId,
                            Amount = x.Amount
                        },
                    DiscountInfo = new DiscountInfo
                        {
                            Sum = x.DiscountSum,
                            Percent = x.DiscountPercent,
                            CalculateDiscountViaPercent = true
                        }
                }).ToArray();

            var beginDistributionDate = orderInfo.EndDistributionDateFact.GetNextMonthFirstDate();
            if (beginDistributionDate <= DateTime.Today)
            {
                beginDistributionDate = DateTime.Today.GetNextMonthFirstDate();
            }

            long priceId;
            if (!_orderRepository.TryGetActualPriceId(orderInfo.DestOrganizationUnitId, beginDistributionDate, out priceId))
            {
                throw new InvalidOperationException(string.Format(BLResources.CannotGetActualPriceListForOrganizationUnit, orderInfo.DestOrganizationUnitId));
            }

            using (var scope = _scopeFactory.CreateNonCoupled<CalculateOrderCostIdentity>())
            {
                var result = _calculateOrderPositionCostService.CalculateOrderPositionsCost(orderInfo.OrderType,
                                                                                            1,
                                                                                            priceId,
                                                                                            orderInfo.SourceOrganizationUnitId,
                                                                                            orderInfo.DestOrganizationUnitId,
                                                                                            orderInfo.FirmId,
                                                                                            positionInfos);

                scope.Complete();
                return result;
            }
        }
    }
}
