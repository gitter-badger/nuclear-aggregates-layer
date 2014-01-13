using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class UpdateOrderFinancialPerformanceHandler : RequestHandler<UpdateOrderFinancialPerformanceRequest, EmptyResponse>
    {
        private readonly IAppSettings _appSettings;
        private readonly IOrderRepository _orderRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public UpdateOrderFinancialPerformanceHandler(
            IOrderRepository orderRepository, 
            IAppSettings appSettings, 
            IOperationScopeFactory scopeFactory)
        {
            _orderRepository = orderRepository;
            _appSettings = appSettings;
            _scopeFactory = scopeFactory;
        }

        protected override EmptyResponse Handle(UpdateOrderFinancialPerformanceRequest request)
        {
            var order = request.Order;

            var orderPositions = _orderRepository.GetPositions(order.Id);

            using (var operationScope = _scopeFactory.CreateNonCoupled<UpdateOrderFinancialPerfomanceIdentity>())
            {
                if (request.RecalculateFromOrder)
                {
                    CalculateDiscountsFromOrder(orderPositions, request, operationScope);
                }
                else
                {
                    CalculateDiscounts(orderPositions, request, operationScope);
                }

                // очищаем причину скидки
                if (order.DiscountSum == 0m || order.DiscountPercent == 0m)
                {
                    order.DiscountReasonEnum = (int)OrderDiscountReason.None;
                    order.DiscountComment = null;
                }

                operationScope.Updated<Order>(order.Id);
                operationScope.Complete();
            }
            
            return Response.Empty;
        }

        private void CalculateDiscounts(IEnumerable<OrderPosition> orderPositions, UpdateOrderFinancialPerformanceRequest request, IOperationScope operationScope)
        {
            var order = request.Order;

            var payablePlanSum = 0m;
            var payablePlanWoVatSum = 0m;
            var payablePriceSum = 0m;
            var orderPositionsPayablePriceWithVatSum = 0m;
            var orderPositionsDiscountSum = 0m;

            foreach (var orderPosition in orderPositions)
            {
                var recalculatedData = _orderRepository.Recalculate(orderPosition.Amount, orderPosition.PricePerUnit, orderPosition.PricePerUnitWithVat, request.ReleaseCountFact, orderPosition.CalculateDiscountViaPercent, orderPosition.DiscountPercent, orderPosition.DiscountSum);

                orderPosition.ShipmentPlan = recalculatedData.ShipmentPlan;
                orderPosition.PayablePrice = recalculatedData.PayablePrice;
                orderPosition.PayablePlan = recalculatedData.PayablePlan;
                orderPosition.PayablePlanWoVat = recalculatedData.PayablePlanWoVat;
                orderPosition.DiscountSum = recalculatedData.DiscountSum;
                orderPosition.DiscountPercent = recalculatedData.DiscountPercent;

                payablePlanSum += orderPosition.PayablePlan;
                payablePlanWoVatSum += orderPosition.PayablePlanWoVat;
                payablePriceSum += orderPosition.PayablePrice;

                orderPositionsPayablePriceWithVatSum += orderPosition.PricePerUnitWithVat * orderPosition.Amount * request.ReleaseCountFact;
                orderPositionsDiscountSum += orderPosition.DiscountSum;

                // Изменяем объект OrderPosition в БД
                _orderRepository.Update(orderPosition);
            }

            operationScope.Updated<OrderPosition>(orderPositions.Select(x => x.Id).ToArray());

            payablePlanSum = Math.Round(payablePlanSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
            payablePlanWoVatSum = Math.Round(payablePlanWoVatSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
            payablePriceSum = Math.Round(payablePriceSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            order.PayablePrice = payablePriceSum;
            order.PayablePlan = payablePlanSum;
            order.PayableFact = payablePlanSum;
            order.VatPlan = payablePlanSum - payablePlanWoVatSum;

            order.DiscountPercent = (orderPositionsPayablePriceWithVatSum == 0m) ? 0m : (orderPositionsDiscountSum * 100m) / orderPositionsPayablePriceWithVatSum;
            order.DiscountSum = orderPositionsDiscountSum;
        }

        private void CalculateDiscountsFromOrder(IList<OrderPosition> orderPositions, UpdateOrderFinancialPerformanceRequest request, IOperationScope operationScope)
        {
            var order = request.Order;
            if (order.DiscountPercent == null || order.DiscountSum == null)
            {
                return;
            }

            if (!orderPositions.Any())
            {
                order.PayablePrice = 0m;
                order.PayablePlan = 0m;
                order.PayableFact = 0m;
                order.VatPlan = 0m;
                order.DiscountPercent = 0m;
                order.DiscountSum = 0m;
                return;
            }

            var payablePlanSum = 0m;
            var payablePlanWoVatSum = 0m;
            var payablePriceSum = 0m;

            var orderPositionsDiscountSum = 0m;

            // пересчитаем процент или сумму, вдруг мы только что удалили позицию 
            var orderPositionsPayablePriceWithVatSum = orderPositions.Sum(orderPosition => orderPosition.PricePerUnitWithVat * orderPosition.Amount * request.ReleaseCountFact);
            if (request.OrderDiscountInPercents)
            {
                order.DiscountSum = orderPositionsPayablePriceWithVatSum * order.DiscountPercent.Value / 100;
            }
            else
            {
                order.DiscountPercent = (orderPositionsPayablePriceWithVatSum == 0m)
                                            ? 0m
                                            : (order.DiscountSum.Value * 100m) / orderPositionsPayablePriceWithVatSum;
            }

            orderPositionsPayablePriceWithVatSum = 0m;
            
            for (var i = 0; i < orderPositions.Count - 1; i++)
            {
                var orderPosition = orderPositions[i];

                // расчёт скидоки для позиции идт через процент скидки заказа, иначе скидка может в минус уйти
                var recalculatedData = _orderRepository.Recalculate(orderPosition.Amount, orderPosition.PricePerUnit, orderPosition.PricePerUnitWithVat, request.ReleaseCountFact, true, order.DiscountPercent.Value, 0);
                orderPosition.ShipmentPlan = recalculatedData.ShipmentPlan;
                orderPosition.PayablePrice = recalculatedData.PayablePrice;
                orderPosition.PayablePlan = recalculatedData.PayablePlan;
                orderPosition.PayablePlanWoVat = recalculatedData.PayablePlanWoVat;
                orderPosition.DiscountSum = recalculatedData.DiscountSum;
                orderPosition.DiscountPercent = recalculatedData.DiscountPercent;
                orderPosition.CalculateDiscountViaPercent = request.OrderDiscountInPercents;

                payablePlanSum += orderPosition.PayablePlan;
                payablePlanWoVatSum += orderPosition.PayablePlanWoVat;
                payablePriceSum += orderPosition.PayablePrice;

                orderPositionsPayablePriceWithVatSum += orderPosition.PricePerUnitWithVat * orderPosition.Amount * request.ReleaseCountFact;
                orderPositionsDiscountSum += orderPosition.DiscountSum;

                // Изменяем объект OrderPosition в БД
                _orderRepository.Update(orderPosition);
            }

            var lastOrderPosition = orderPositions[orderPositions.Count - 1];

            // последнюю позицию надо рассчитать не в процентах, чтобы сохранить точность
            var recalculatedDataForLastOrderPosition = _orderRepository.Recalculate(lastOrderPosition.Amount, lastOrderPosition.PricePerUnit, lastOrderPosition.PricePerUnitWithVat, request.ReleaseCountFact, false, order.DiscountPercent.Value, order.DiscountSum.Value - orderPositionsDiscountSum);
            lastOrderPosition.ShipmentPlan = recalculatedDataForLastOrderPosition.ShipmentPlan;
            lastOrderPosition.PayablePrice = recalculatedDataForLastOrderPosition.PayablePrice;
            lastOrderPosition.PayablePlan = recalculatedDataForLastOrderPosition.PayablePlan;
            lastOrderPosition.PayablePlanWoVat = recalculatedDataForLastOrderPosition.PayablePlanWoVat;
            lastOrderPosition.DiscountSum = recalculatedDataForLastOrderPosition.DiscountSum;
            lastOrderPosition.DiscountPercent = recalculatedDataForLastOrderPosition.DiscountPercent;
            lastOrderPosition.CalculateDiscountViaPercent = request.OrderDiscountInPercents;

            // Изменяем объект OrderPosition в БД
            _orderRepository.Update(lastOrderPosition);

            operationScope.Updated<OrderPosition>(orderPositions.Select(x => x.Id).ToArray());

            payablePlanSum += lastOrderPosition.PayablePlan;
            payablePlanWoVatSum += lastOrderPosition.PayablePlanWoVat;
            payablePriceSum += lastOrderPosition.PayablePrice;
            orderPositionsPayablePriceWithVatSum += lastOrderPosition.PricePerUnitWithVat * lastOrderPosition.Amount * request.ReleaseCountFact;
            orderPositionsDiscountSum += lastOrderPosition.DiscountSum;

            payablePlanSum = Math.Round(payablePlanSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
            payablePlanWoVatSum = Math.Round(payablePlanWoVatSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);
            payablePriceSum = Math.Round(payablePriceSum, _appSettings.SignificantDigitsNumber, MidpointRounding.ToEven);

            order.PayablePrice = payablePriceSum;
            order.PayablePlan = payablePlanSum;
            order.PayableFact = payablePlanSum;
            order.VatPlan = payablePlanSum - payablePlanWoVatSum;

            order.DiscountPercent = (orderPositionsPayablePriceWithVatSum == 0m) ? 0m : (orderPositionsDiscountSum * 100m) / orderPositionsPayablePriceWithVatSum;
            order.DiscountSum = orderPositionsDiscountSum;
        }
    }
}
