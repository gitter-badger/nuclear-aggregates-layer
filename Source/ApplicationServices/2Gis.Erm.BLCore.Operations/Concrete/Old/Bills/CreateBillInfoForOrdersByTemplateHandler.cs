using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills
{
    public sealed class CreateBillInfoForOrdersByTemplateHandler 
        : RequestHandler<CreateBillInfoForOrdersByTemplateRequest, CreateBillInfoForOrdersByTemplateResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public CreateBillInfoForOrdersByTemplateHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override CreateBillInfoForOrdersByTemplateResponse Handle(CreateBillInfoForOrdersByTemplateRequest request)
        {
            // вытаскиваем данные сколько по каждому из заказов к оплате план
            var ordersInfos = _orderRepository.GetPayablePlans(request.OrderIds).ToArray();

            // опеределяем долю каждого платежа в шаблоне платежей от общей суммы
            var totalPayablePlan = request.CreateBillInfosTemplate.Sum(i => i.PayablePlan);
            if (totalPayablePlan == 0 && request.CreateBillInfosTemplate.Length > 1)
            {   // т.е. создание счетов по шаблону заказа с нулевой суммой к оплате план + с использованием рассрочки запрещено
                // чтобы не формировались платежи для связанных заказов, с не нулевой суммой, в который вся сумма к оплате просталвется в последний платеж
                throw new NotificationException(BLResources.BillMassCreateOrderHasZeroPayablePlan);
            }

            var shares = new decimal[request.CreateBillInfosTemplate.Length];
            for (int i = 0; i < request.CreateBillInfosTemplate.Length; i++)
            {
                shares[i] = totalPayablePlan != 0m ? 
                                request.CreateBillInfosTemplate[i].PayablePlan / totalPayablePlan : 0;
            }

            // расчитываем платежи для заказов, так чтобы в каждом заказе каждый платеж имел ту же долю в оплате план заказа, что и в шаблоне платежей
            var orderBillsByTempalte = new Tuple<long, CreateBillInfo[]>[ordersInfos.Length];
            for (int i = 0; i < ordersInfos.Length; i++)
            {
                var billInfos = new CreateBillInfo[request.CreateBillInfosTemplate.Length];
                decimal paymentsSum = 0m;

                // обрабатываем все платежи кроме последнего
                for (int j = 0; j < request.CreateBillInfosTemplate.Length - 1; j++)
                {
                    billInfos[j] = new CreateBillInfo
                                       {
                                           PaymentNumber = request.CreateBillInfosTemplate[j].PaymentNumber,
                                           PaymentDatePlan = request.CreateBillInfosTemplate[j].PaymentDatePlan,
                                           BeginDistributionDate = request.CreateBillInfosTemplate[j].BeginDistributionDate,
                                           EndDistributionDate = request.CreateBillInfosTemplate[j].EndDistributionDate,
                                           PayablePlan = Math.Round(ordersInfos[i].PayablePlan * shares[j], 2, MidpointRounding.ToEven)
                                       };
                    paymentsSum += billInfos[j].PayablePlan;
                }

                // рассчитаем последний платеж, добиваясь точного совпадения суммы всех платежей с числом к оплате план заказа
                var lastIndex = request.CreateBillInfosTemplate.Length - 1;
                billInfos[lastIndex] = new CreateBillInfo
                                           {
                                               PaymentNumber = request.CreateBillInfosTemplate[lastIndex].PaymentNumber,
                                               PaymentDatePlan = request.CreateBillInfosTemplate[lastIndex].PaymentDatePlan,
                                               BeginDistributionDate = request.CreateBillInfosTemplate[lastIndex].BeginDistributionDate,
                                               EndDistributionDate = request.CreateBillInfosTemplate[lastIndex].EndDistributionDate,
                                               PayablePlan = ordersInfos[i].PayablePlan - paymentsSum
                                           };

                orderBillsByTempalte[i] = new Tuple<long, CreateBillInfo[]>(ordersInfos[i].OrderId, billInfos);
            }

            return new CreateBillInfoForOrdersByTemplateResponse { OrdersCreateBillInfos = orderBillsByTempalte }; 
        }
    }
}
