using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Positions.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders
{
    public sealed class ActualizeOrderReleaseWithdrawalsHandler : RequestHandler<ActualizeOrderReleaseWithdrawalsRequest, EmptyResponse>
    {
        private readonly IPositionReadModel _positionReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderDeleteReleaseWithdrawalsAggregateService _deleteReleaseWithdrawalsAggregateService;
        private readonly IOrderDeleteReleaseTotalsAggregateService _deleteReleaseTotalsAggregateService;
        private readonly IOrderCreateReleaseTotalsAggregateService _createReleaseTotalsAggregateService;
        private readonly IOrderCreateReleaseWithdrawalsAggregateService _createReleaseWithdrawalsAggregateService;
        private readonly IOrderActualizeAmountToWithdrawAggregateService _actualizeAmountToWithdrawAggregateService;
        private readonly IPriceCostsForSubPositionsProvider _priceCostsForSubPositionsProvider;
        private readonly IPaymentsDistributor _paymentsDistributor;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ActualizeOrderReleaseWithdrawalsHandler(
            IPositionReadModel positionReadModel,
            IOrderReadModel orderReadModel,
            IOrderDeleteReleaseWithdrawalsAggregateService deleteReleaseWithdrawalsAggregateService,
            IOrderDeleteReleaseTotalsAggregateService deleteReleaseTotalsAggregateService,
            IOrderCreateReleaseTotalsAggregateService createReleaseTotalsAggregateService,
            IOrderCreateReleaseWithdrawalsAggregateService createReleaseWithdrawalsAggregateService,
            IOrderActualizeAmountToWithdrawAggregateService actualizeAmountToWithdrawAggregateService,
            IPriceCostsForSubPositionsProvider priceCostsForSubPositionsProvider,           
            IPaymentsDistributor paymentsDistributor,
            IPublicService publicService,
            IOperationScopeFactory scopeFactory)
        {
            _positionReadModel = positionReadModel;
            _orderReadModel = orderReadModel;
            _deleteReleaseWithdrawalsAggregateService = deleteReleaseWithdrawalsAggregateService;
            _deleteReleaseTotalsAggregateService = deleteReleaseTotalsAggregateService;
            _createReleaseTotalsAggregateService = createReleaseTotalsAggregateService;
            _createReleaseWithdrawalsAggregateService = createReleaseWithdrawalsAggregateService;
            _actualizeAmountToWithdrawAggregateService = actualizeAmountToWithdrawAggregateService;
            _priceCostsForSubPositionsProvider = priceCostsForSubPositionsProvider;
            _paymentsDistributor = paymentsDistributor;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(ActualizeOrderReleaseWithdrawalsRequest request)
        {
            var order = request.Order;

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeOrderReleaseWithdrawalsIdentity>())
            {
                var orderInfo = _orderReadModel.GetOrderRecalculateWithdrawalsInfo(order.Id);
                _deleteReleaseWithdrawalsAggregateService.Delete(orderInfo.OrderPositions.SelectMany(x => x.ReleaseWithdrawals));
                _deleteReleaseTotalsAggregateService.Delete(orderInfo.ReleaseTotals);

                IReadOnlyCollection<OrderReleaseTotal> orderReleaseTotals = new OrderReleaseTotal[0];
                
                // ≈сли это заказ, закрытый отказом, то записи пересоздавать не надо
                if (order.IsActive)
                {
                    var releaseWithdrawalsForOrder = new List<OrderReleaseWithdrawalDto>();
                    foreach (var orderPosition in orderInfo.OrderPositions)
                    {
                        var releaseWithdrawals = CalculateReleaseWithdrawals(order, orderPosition);
                        releaseWithdrawalsForOrder.AddRange(releaseWithdrawals);
                    }

                    _createReleaseWithdrawalsAggregateService.Create(releaseWithdrawalsForOrder);

                    orderReleaseTotals = CalculateOrderReleaseTotals(order, releaseWithdrawalsForOrder.Select(x => x.WidrawalInfo).ToArray());
                    _createReleaseTotalsAggregateService.Create(orderReleaseTotals);
                }

                var amountToWithdraw = order.WorkflowStepId != OrderState.Archive
                    ? orderReleaseTotals.Where(x => x.ReleaseNumber == order.BeginReleaseNumber
                                                                        + orderInfo.LocksCount).Select(x => x.AmountToWithdraw).FirstOrDefault()
                    : 0m;

                _actualizeAmountToWithdrawAggregateService.Actualize(order, amountToWithdraw);

                scope.Complete();
            }

            return Response.Empty;
        }

        private IEnumerable<OrderReleaseWithdrawalDto> CalculateReleaseWithdrawals(
            Order order,
            OrderRecalculateWithdrawalsDto.OrderPositionDto orderPosition)
        {
            var paymentPlanDistribution = _paymentsDistributor.DistributePayment(order.ReleaseCountPlan, orderPosition.PayablePlan);
            var vatDistribution = _paymentsDistributor.DistributePayment(order.ReleaseCountPlan, orderPosition.PayablePlan - orderPosition.PayablePlanWoVat);

            var releaseWithdrawals = new List<ReleaseWithdrawal>();

            // √отовим списани€
            for (var release = order.BeginReleaseNumber; release <= order.EndReleaseNumberFact; release++)
            {
                var index = release - order.BeginReleaseNumber;

                var releaseBeginDate = release == order.BeginReleaseNumber
                                           ? order.BeginDistributionDate
                                           : order.BeginDistributionDate.AddMonths(index).GetFirstDateOfMonth();

                var releaseEndDate = release == order.EndReleaseNumberPlan
                                         ? order.EndDistributionDatePlan
                                         : releaseBeginDate.GetEndPeriodOfThisMonth();

                releaseWithdrawals.Add(new ReleaseWithdrawal
                {
                    OrderPositionId = orderPosition.Id,
                    ReleaseNumber = release,
                    ReleaseBeginDate = releaseBeginDate,
                    ReleaseEndDate = releaseEndDate,

                    AmountToWithdraw = paymentPlanDistribution[index],
                    Vat = vatDistribution[index]
                });
            }

            return CalculateReleaseWithdrawalPositions(orderPosition, releaseWithdrawals);
        }

        private IEnumerable<OrderReleaseWithdrawalDto> CalculateReleaseWithdrawalPositions(
            OrderRecalculateWithdrawalsDto.OrderPositionDto orderPosition,
            IEnumerable<ReleaseWithdrawal> releaseWithdrawals)
        {
            // ѕровер€ем €вл€етс€ ли позици€ пакетом
            if (!orderPosition.IsComposite)
            {
                return PrepareReleaseWithdrawalPositionsForSimplePosition(orderPosition.PlatformId, orderPosition.PositionId, releaseWithdrawals);
            }

            var priceConstInfos = _priceCostsForSubPositionsProvider.GetPriceCostsForSubPositions(orderPosition.PositionId, orderPosition.PriceId);
            if (priceConstInfos.Count > 0)
            {   // пакет разбиваетс€ MoDi-сервисом
                return PrepareReleaseWithdrawalPositionsForCompositePositionWithMoDi(
                            orderPosition.OrderId,
                            orderPosition.Amount,
                            orderPosition.CategoryRate,
                            orderPosition.DiscountSum,
                            orderPosition.DiscountPercent,
                            priceConstInfos, 
                            releaseWithdrawals.ToArray());
            }
            
            var subPositions = _orderReadModel.GetSelectedSubPositions(orderPosition.Id).ToArray();
            return PrepareReleaseWithdrawalPositionsForCompositePositionWithoutMoDi(subPositions, releaseWithdrawals);
        }

        private IReadOnlyCollection<OrderReleaseTotal> CalculateOrderReleaseTotals(Order order, IReadOnlyCollection<ReleaseWithdrawal> releaseWithdrawals)
        {
            var orderReleaseTotals = new List<OrderReleaseTotal>();
            for (var i = order.BeginReleaseNumber; i <= order.EndReleaseNumberFact; i++)
            {
                var releaseNumber = i;
                var months = releaseNumber - order.BeginReleaseNumber;

                var releaseBeginDate = releaseNumber == order.BeginReleaseNumber ?
                                                                order.BeginDistributionDate :
                                                                order.BeginDistributionDate.AddMonths(months).GetFirstDateOfMonth();

                var releaseEndDate = releaseNumber == order.EndReleaseNumberPlan ?
                                                                order.EndDistributionDatePlan :
                                                                releaseBeginDate.GetEndPeriodOfThisMonth();

                var amountToWithdraw = releaseWithdrawals.Where(x => x.ReleaseNumber == releaseNumber).Sum(x => x.AmountToWithdraw);
                var vat = releaseWithdrawals.Where(x => x.ReleaseNumber == releaseNumber).Sum(x => x.Vat);

                if (amountToWithdraw == 0 && vat == 0)
                {
                    continue;
                }

                var total = new OrderReleaseTotal
                {
                    OrderId = order.Id,
                    ReleaseNumber = releaseNumber,
                    ReleaseBeginDate = releaseBeginDate,
                    ReleaseEndDate = releaseEndDate,
                    AmountToWithdraw = amountToWithdraw,
                    Vat = vat
                };

                orderReleaseTotals.Add(total);
            }

            return orderReleaseTotals;
        }

        private IEnumerable<OrderReleaseWithdrawalDto> PrepareReleaseWithdrawalPositionsForSimplePosition(
            long platformId,
            long positionId,
            IEnumerable<ReleaseWithdrawal> releaseWithdrawals)
        {
            return releaseWithdrawals
                        .Select(releaseWithdrawal => 
                            new OrderReleaseWithdrawalDto
                                {
                                    WidrawalInfo = releaseWithdrawal,
                                    WithdrawalsPositions = new[]
                                                            {
                                                                new ReleasesWithdrawalsPosition
                                                                {
                                                                    AmountToWithdraw = releaseWithdrawal.AmountToWithdraw,
                                                                    PlatformId = platformId,
                                                                    PositionId = positionId,
                                                                    Vat = releaseWithdrawal.Vat
                                                                } 
                                                            }
                                })
                        .ToArray();
        }

        private IEnumerable<OrderReleaseWithdrawalDto> PrepareReleaseWithdrawalPositionsForCompositePositionWithoutMoDi(
            IReadOnlyList<SubPositionDto> subPositions,
            IEnumerable<ReleaseWithdrawal> releaseWithdrawals)
        {
            var result = new List<OrderReleaseWithdrawalDto>();
            foreach (var releaseWithdrawal in releaseWithdrawals)
            {
                var witdrawalPositions = new List<ReleasesWithdrawalsPosition>();
                var positionsPaymentDistributions = _paymentsDistributor.DistributePayment(subPositions.Count(), releaseWithdrawal.AmountToWithdraw);
                var vatPositionsPaymentDistributions = _paymentsDistributor.DistributePayment(subPositions.Count(), releaseWithdrawal.Vat);

                for (int subPositionIndex = 0; subPositionIndex < subPositions.Count(); subPositionIndex++)
                {
                    var witdrawalPosition = new ReleasesWithdrawalsPosition
                        {
                            AmountToWithdraw = positionsPaymentDistributions[subPositionIndex],
                            PlatformId = subPositions[subPositionIndex].PlatformId,
                            PositionId = subPositions[subPositionIndex].PositionId,
                            Vat = vatPositionsPaymentDistributions[subPositionIndex]
                        };

                    witdrawalPositions.Add(witdrawalPosition);
                }

                result.Add(new OrderReleaseWithdrawalDto
                               {
                                   WidrawalInfo = releaseWithdrawal, 
                                   WithdrawalsPositions = witdrawalPositions
                               });
            }

            return result;
        }

        private IEnumerable<OrderReleaseWithdrawalDto> PrepareReleaseWithdrawalPositionsForCompositePositionWithMoDi(
            long orderId,
            int orderPositionAmount,
            decimal categoryRate,
            decimal discountSum,
            decimal discountPercent,
            IReadOnlyCollection<PriceCostDto> priceCostInfos,
            IReadOnlyList<ReleaseWithdrawal> releaseWithdrawals)
        {
            var payablePlans = new Dictionary<PriceCostDto, decimal>();
            var vats = new Dictionary<PriceCostDto, decimal>();
            var payablePlansDistributions = new Dictionary<PriceCostDto, decimal[]>();
            var vatsDistributions = new Dictionary<PriceCostDto, decimal[]>();
            
            foreach (var costInfo in priceCostInfos)
            {
                var response = (CalculateOrderPositionPricesResponse)_publicService.Handle(new CalculateOrderPositionPricesRequest
                    {
                        Amount = orderPositionAmount,
                        OrderId = orderId,
                        CalculateDiscountViaPercent = true,
                        Cost = costInfo.Cost,
                        CategoryRate = categoryRate,
                        DiscountSum = discountSum,
                        DiscountPercent = discountPercent
                    });

                payablePlans.Add(costInfo, response.PayablePlan);
                vats.Add(costInfo, response.PayablePlan - response.PayablePlanWoVat);

                var positionsPaymentDistributions = _paymentsDistributor.DistributePayment(releaseWithdrawals.Count, payablePlans[costInfo]);
                var vatPositionsPaymentDistributions = _paymentsDistributor.DistributePayment(releaseWithdrawals.Count, vats[costInfo]);

                payablePlansDistributions.Add(costInfo, positionsPaymentDistributions);
                vatsDistributions.Add(costInfo, vatPositionsPaymentDistributions);
            }

            var platformDgppIds = priceCostInfos.Select(x => (long)x.Platform).ToArray();
            var platformMap = _positionReadModel.GetPlatformsDictionary(platformDgppIds);

            var result = new List<OrderReleaseWithdrawalDto>();
            for (var i = 0; i < releaseWithdrawals.Count; i++)
            {
                var releaseWithdrawal = releaseWithdrawals[i];
                var withdrawalPositions = new List<ReleasesWithdrawalsPosition>();
                foreach (var costInfo in priceCostInfos)
                {
                    withdrawalPositions.Add(new ReleasesWithdrawalsPosition
                        {
                            AmountToWithdraw = payablePlansDistributions[costInfo][i],
                            PlatformId = platformMap[costInfo.Platform],
                            PositionId = costInfo.PositionId,
                            Vat = vatsDistributions[costInfo][i],
                        });
                }

                result.Add(new OrderReleaseWithdrawalDto
                    {
                        WidrawalInfo = releaseWithdrawal,
                        WithdrawalsPositions = withdrawalPositions
                    });
            }

            return result;
        }
    }
}
