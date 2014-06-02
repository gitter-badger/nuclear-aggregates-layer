using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders
{
    // TODO {all, 29.05.2014}: при рефакторинге ApplicationService учесть наличие клона MultiCulture - нужно их максимально объединить
    public sealed class CalculateReleaseWithdrawalsHandler : RequestHandler<CalculateReleaseWithdrawalsRequest, EmptyResponse>, ICzechAdapted, ICyprusAdapted, IChileAdapted, IUkraineAdapted
    {
        private readonly IFinder _finder;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublicService _publicService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPaymentsDistributor _paymentsDistributor;

        public CalculateReleaseWithdrawalsHandler(
            IFinder finder,
            IUnitOfWork unitOfWork,
            IPublicService publicService,
            IOperationScopeFactory scopeFactory,
            IOrderReadModel orderReadModel,
            IPaymentsDistributor paymentsDistributor)
        {
            _finder = finder;
            _unitOfWork = unitOfWork;
            _publicService = publicService;
            _scopeFactory = scopeFactory;
            _orderReadModel = orderReadModel;
            _paymentsDistributor = paymentsDistributor;
        }

        protected override EmptyResponse Handle(CalculateReleaseWithdrawalsRequest request)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<CalculateReleaseWithdrawalsIdentity>())
            {
                if (request.UpdateAmountToWithdrawOnly)
                {
                    UpdateOrderAmountToWithdrawOnly(request.Order, scope);
                }
                else
                {
                    CalculateReleaseWithdrawals(request.Order, scope);
                }

                scope.Complete();
            }

            return Response.Empty;
        }

        /// <summary>
        /// Пересчитать списания (ReleaseWithdrawals и ReleaseTotals) и поле AmountToWithdraw для указанного заказа.
        /// </summary>
        private void CalculateReleaseWithdrawals(Order order, IOperationScope operationScope)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(order.Id))
                                   .Select(x => new
                                   {
                                       LocksCount = x.Locks.Count(@lock => !@lock.IsDeleted && !@lock.IsActive),
                                       OrderPositions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                                         .Select(y => new OrderPositionDto
                                                         {
                                                             Id = y.Id,
                                                             PayablePlan = y.PayablePlan,
                                                             PayablePlanWoVat = y.PayablePlanWoVat,
                                                             PlatformId = y.PricePosition.Position.PlatformId,
                                                             PriceId = y.PricePosition.PriceId,
                                                             OrderId = y.OrderId,
                                                             PositionId = y.PricePosition.PositionId,
                                                             Amount = y.Amount,
                                                             DiscountSum = y.DiscountSum,
                                                             DiscountPercent = y.DiscountPercent,
                                                             CalculateDiscountViaPercent = y.CalculateDiscountViaPercent,
                                                             CategoryRate = y.CategoryRate,
                                                             IsComposite = y.PricePosition.Position.IsComposite
                                                         })
                                   })
                                   .Single();

            foreach (var orderPosition in orderInfo.OrderPositions)
            {
                PrepareWithdrawalPositionsInfo(orderPosition);
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var withdrawalRepository = scope.CreateRepository<IWithdrawalInfoRepository>();
                var deletedIds = withdrawalRepository.DeleteReleaseWithdrawalPositionsForOrder(order.Id);
                operationScope.Deleted<ReleasesWithdrawalsPosition>(deletedIds);
                scope.Complete();
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var withdrawalRepository = scope.CreateRepository<IWithdrawalInfoRepository>();
                var deletedReleaseWithdrawals = withdrawalRepository.DeleteReleaseWithdrawalsForOrder(order.Id);
                operationScope.Deleted<ReleaseWithdrawal>(deletedReleaseWithdrawals);

                var orderRepository = scope.CreateRepository<IOrderRepository>();
                var deletedOrderReleaseTotals = orderRepository.DeleteOrderReleaseTotalsForOrder(order.Id);
                operationScope.Deleted<OrderReleaseTotal>(deletedOrderReleaseTotals);

                // Если это заказ, закрытый отказом, то записи пересоздавать не надо
                IEnumerable<ReleaseWithdrawal> allReleaseWithdrawals = Enumerable.Empty<ReleaseWithdrawal>();
                IEnumerable<OrderReleaseTotal> orderReleaseTotals = Enumerable.Empty<OrderReleaseTotal>();

                if (order.IsActive)
                {
                    foreach (var orderPosition in orderInfo.OrderPositions)
                    {
                        var releaseWithdrawals = CreateReleaseWithdrawalsForOrderPosition(order, orderPosition).ToArray();
                        withdrawalRepository.Create(releaseWithdrawals);
                        operationScope.Added<ReleaseWithdrawal>(releaseWithdrawals.Select(withdrawal => withdrawal.Id).ToArray());

                        orderPosition.ReleaseWithdrawals = releaseWithdrawals;

                        allReleaseWithdrawals = allReleaseWithdrawals.Union(releaseWithdrawals);
                    }

                    orderReleaseTotals = CreateOrderReleaseTotals(order, allReleaseWithdrawals);
                    orderRepository.CreateOrderReleaseTotals(orderReleaseTotals);
                    operationScope.Added<OrderReleaseTotal>(orderReleaseTotals.Select(total => total.Id).ToArray());
                }

                var amountToWithdraw = 0m;
                if (order.WorkflowStepId != (int)OrderState.Archive)
                {
                    amountToWithdraw = orderReleaseTotals.Where(x => x.ReleaseNumber == order.BeginReleaseNumber
                                                                     + orderInfo.LocksCount).Select(x => x.AmountToWithdraw).FirstOrDefault();
                }

                order.AmountToWithdraw = amountToWithdraw;

                // Заказ пришел из вызывающего хендлера - ему заказ и сохранять.
                // var orderRepository = scope.CreateRepository<OrderRepository>();
                // orderRepository.Update(order);

                // TODO проверить -  Объект не удаляем из контекста т.к. потом объект ещё используется (изменяется и сохраняется)  выше по дереву вызова.
                scope.Complete();
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var withdrawalRepository = scope.CreateRepository<IWithdrawalInfoRepository>();
                var orderRepository = scope.CreateRepository<IOrderRepository>();

                var releaseWithdrawalPositions = CreateReleaseWithdrawalPositions(_orderReadModel, orderInfo.OrderPositions);
                withdrawalRepository.Create(releaseWithdrawalPositions);
                operationScope.Added<ReleasesWithdrawalsPosition>(releaseWithdrawalPositions.Select(position => position.Id).ToArray());

                scope.Complete();
            }
        }

        private void UpdateOrderAmountToWithdrawOnly(Order order, IOperationScope operationScope)
        {
            if (order.WorkflowStepId == (int)OrderState.Archive)
            {
                order.AmountToWithdraw = 0m;
            }
            else
            {
                var orderReleaseTotalQuery = _finder.FindAll<OrderReleaseTotal>();

                order.AmountToWithdraw =
                    orderReleaseTotalQuery.Where(x => x.OrderId == order.Id)
                        .Select(oe => oe.Order.OrderReleaseTotals
                                          .Where(
                                              orderReleaseTotal =>
                                              orderReleaseTotal.ReleaseNumber ==
                                              oe.Order.BeginReleaseNumber +
                                              oe.Order.Locks.Count(@lock => !@lock.IsDeleted && !@lock.IsActive))
                                          .Select(orderReleaseTotal => orderReleaseTotal.AmountToWithdraw)
                                          .FirstOrDefault())
                        .FirstOrDefault();
            }

            using (var scope = _unitOfWork.CreateScope())
            {
                var orderRepository = scope.CreateRepository<IOrderRepository>();
                orderRepository.Update(order);
                operationScope.Updated<Order>(order.Id);
                scope.Complete();
            }
        }

        private IEnumerable<OrderReleaseTotal> CreateOrderReleaseTotals(Order orderBatch, IEnumerable<ReleaseWithdrawal> allWithdrawals)
        {
            var orderReleaseTotals = new List<OrderReleaseTotal>();
            for (var i = orderBatch.BeginReleaseNumber; i <= orderBatch.EndReleaseNumberFact; i++)
            {
                var releaseNumber = i;
                var months = releaseNumber - orderBatch.BeginReleaseNumber;

                var releaseBeginDate = releaseNumber == orderBatch.BeginReleaseNumber ?
                                                                orderBatch.BeginDistributionDate :
                                                                orderBatch.BeginDistributionDate.AddMonths(months).GetFirstDateOfMonth();

                var releaseEndDate = releaseNumber == orderBatch.EndReleaseNumberPlan ?
                                                                orderBatch.EndDistributionDatePlan :
                                                                releaseBeginDate.GetEndPeriodOfThisMonth();

                var amountToWithdraw = allWithdrawals.Where(x => x.ReleaseNumber == releaseNumber).Sum(x => x.AmountToWithdraw);
                var vat = allWithdrawals.Where(x => x.ReleaseNumber == releaseNumber).Sum(x => x.Vat);

                if (amountToWithdraw == 0 && vat == 0)
                {
                    continue;
                }

                var total = new OrderReleaseTotal
                {
                    OrderId = orderBatch.Id,
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

        private void PrepareWithdrawalPositionsInfo(OrderPositionDto orderPosition)
        {
            orderPosition.PriceCostInfos = new PriceCostInfo[0];
        }

        private IEnumerable<ReleasesWithdrawalsPosition> CreateReleaseWithdrawalPositions(IOrderReadModel orderReadModel, IEnumerable<OrderPositionDto> orderPosition)
        {
            var result = new List<ReleasesWithdrawalsPosition>();
            foreach (var orderPositionDto in orderPosition)
            {
                // Проверяем является ли позиция пакетом
                if (!orderPositionDto.IsComposite)
                {
                    result.AddRange(CreateReleaseWithdrawalPositionsForSimplePosition(orderPositionDto));
                }

                // Проверяем разбивается ли пакет MoDi-сервисом
                else if (orderPositionDto.PriceCostInfos.Length > 0)
                {
                    result.AddRange(CreateReleaseWithdrawalPositionsForCompositePositionWithMoDi(orderPositionDto));
                }
                else
                {
                    var subPositions = orderReadModel.GetSelectedSubPositions(orderPositionDto.Id).ToArray();
                    result.AddRange(CreateReleaseWithdrawalPositionsForCompositePositionWithoutMoDi(subPositions, orderPositionDto));
                }
            }

            return result;
        }

        private IEnumerable<ReleasesWithdrawalsPosition> CreateReleaseWithdrawalPositionsForSimplePosition(OrderPositionDto orderPosition)
        {
            return orderPosition.ReleaseWithdrawals
                                .Select(releaseWithdrawal => new ReleasesWithdrawalsPosition
                                {
                                    AmountToWithdraw = releaseWithdrawal.AmountToWithdraw,
                                    PlatformId = orderPosition.PlatformId,
                                    PositionId = orderPosition.PositionId,
                                    ReleasesWithdrawalId = releaseWithdrawal.Id,
                                    Vat = releaseWithdrawal.Vat
                                })
                                .ToArray();
        }

        private IEnumerable<ReleasesWithdrawalsPosition> CreateReleaseWithdrawalPositionsForCompositePositionWithoutMoDi(SubPositionDto[] subPositions, OrderPositionDto orderPosition)
        {
            var result = new List<ReleasesWithdrawalsPosition>();
            foreach (var releaseWithdrawal in orderPosition.ReleaseWithdrawals)
            {
                var positionsPaymentDistributions = _paymentsDistributor.DistributePayment(subPositions.Count(), releaseWithdrawal.AmountToWithdraw);
                var vatPositionsPaymentDistributions = _paymentsDistributor.DistributePayment(subPositions.Count(), releaseWithdrawal.Vat);

                for (int subPositionIndex = 0; subPositionIndex < subPositions.Count(); subPositionIndex++)
                {
                    var newReleasesWithdrawalsPositionItem = new ReleasesWithdrawalsPosition
                    {
                        AmountToWithdraw = positionsPaymentDistributions[subPositionIndex],
                        PlatformId = subPositions[subPositionIndex].PlatformId,
                        PositionId = subPositions[subPositionIndex].PositionId,
                        ReleasesWithdrawalId = releaseWithdrawal.Id,
                        Vat = vatPositionsPaymentDistributions[subPositionIndex]
                    };

                    result.Add(newReleasesWithdrawalsPositionItem);
                }
            }

            return result;
        }

        private IEnumerable<ReleasesWithdrawalsPosition> CreateReleaseWithdrawalPositionsForCompositePositionWithMoDi(OrderPositionDto orderPosition)
        {
            var payablePlans = new Dictionary<PriceCostInfo, decimal>();
            var vats = new Dictionary<PriceCostInfo, decimal>();
            var payablePlansDistributions = new Dictionary<PriceCostInfo, decimal[]>();
            var vatsDistributions = new Dictionary<PriceCostInfo, decimal[]>();
            foreach (var costInfo in orderPosition.PriceCostInfos)
            {
                var response = (CalculateOrderPositionPricesResponse)_publicService.Handle(new CalculateOrderPositionPricesRequest
                {
                    Amount = orderPosition.Amount,
                    OrderId = orderPosition.OrderId,
                    CalculateDiscountViaPercent = true,
                    Cost = costInfo.Cost,
                    CategoryRate = orderPosition.CategoryRate,
                    DiscountSum = orderPosition.DiscountSum,
                    DiscountPercent = orderPosition.DiscountPercent
                });

                payablePlans.Add(costInfo, response.PayablePlan);
                vats.Add(costInfo, response.PayablePlan - response.PayablePlanWoVat);

                var positionsPaymentDistributions = _paymentsDistributor.DistributePayment(orderPosition.ReleaseWithdrawals.Count(), payablePlans[costInfo]);
                var vatPositionsPaymentDistributions = _paymentsDistributor.DistributePayment(orderPosition.ReleaseWithdrawals.Count(), vats[costInfo]);

                payablePlansDistributions.Add(costInfo, positionsPaymentDistributions);
                vatsDistributions.Add(costInfo, vatPositionsPaymentDistributions);
            }

            var platformDgppIds = orderPosition.PriceCostInfos.Select(x => (long)x.Platform).ToArray();
            var platformMap = _finder.Find<Platform.Model.Entities.Erm.Platform>(x => platformDgppIds.Contains(x.DgppId)).ToDictionary(x => (PlatformEnum)x.DgppId, x => x.Id);

            var result = new List<ReleasesWithdrawalsPosition>();
            for (var i = 0; i < orderPosition.ReleaseWithdrawals.Count(); i++)
            {
                foreach (var costInfo in orderPosition.PriceCostInfos)
                {
                    result.Add(new ReleasesWithdrawalsPosition
                    {
                        AmountToWithdraw = payablePlansDistributions[costInfo][i],
                        PlatformId = platformMap[costInfo.Platform],
                        PositionId = costInfo.PositionId,
                        ReleasesWithdrawalId = orderPosition.ReleaseWithdrawals[i].Id,
                        Vat = vatsDistributions[costInfo][i],
                    });
                }
            }

            return result;
        }

        private IEnumerable<ReleaseWithdrawal> CreateReleaseWithdrawalsForOrderPosition(
            Order order,
            OrderPositionDto orderPosition)
        {
            var paymentPlanDistribution = _paymentsDistributor.DistributePayment(order.ReleaseCountPlan, orderPosition.PayablePlan);
            var vatDistribution = _paymentsDistributor.DistributePayment(order.ReleaseCountPlan, orderPosition.PayablePlan - orderPosition.PayablePlanWoVat);

            var releaseWithdrawals = new List<ReleaseWithdrawal>();

            // Готовим списания
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

            return releaseWithdrawals;
        }

        #region nested types

        private sealed class OrderPositionDto
        {
            public long Id { get; set; }
            public decimal PayablePlan { get; set; }
            public decimal PayablePlanWoVat { get; set; }
            public bool IsComposite { get; set; }
            public long PlatformId { get; set; }
            public long OrderId { get; set; }
            public int Amount { get; set; }
            public decimal DiscountSum { get; set; }
            public decimal DiscountPercent { get; set; }
            public bool CalculateDiscountViaPercent { get; set; }
            public PriceCostInfo[] PriceCostInfos { get; set; }
            public ReleaseWithdrawal[] ReleaseWithdrawals { get; set; }
            public long PositionId { get; set; }
            public long PriceId { get; set; }
            public decimal CategoryRate { get; set; }
        }

        #endregion
    }
}