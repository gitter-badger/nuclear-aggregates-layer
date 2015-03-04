using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class BillController : ControllerBase
    {
        // для первого платежа - 20 число месяца, для последующих - 10 число месяца
        // Если Дата подписания > 20-го числа текущего месяца то "Дата оплаты, до" в первом (единственном) счёте устанавливать = Дате подписания БЗ.
        private static readonly Func<int, DateTime, DateTime, DateTime> PaymentDatePlanEvaluator =
            (paymentNumber, signupDate, beginPeriod) =>
                {
                    if (paymentNumber == 1)
                    {
                        var firstPaymantDate = beginPeriod.AddMonths(-1).AddDays(20 - beginPeriod.Day);
                        return signupDate.Day > 20 && signupDate.Month == firstPaymantDate.Month && signupDate.Year == firstPaymantDate.Year ? signupDate : firstPaymantDate;
                    }

                    return beginPeriod.AddMonths(-1).AddDays(10 - beginPeriod.Day);
                };

        private readonly IPublicService _publicService;
        private readonly IDeleteOrderBillsOperationService _deleteBillsService;
        private readonly ICreateOrderBillsOperationService _createOrderBillsService;

        public BillController(IMsCrmSettings msCrmSettings,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IAPIIdentityServiceSettings identityServiceSettings,
                              IUserContext userContext,
                              ITracer logger,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IPublicService publicService,
                              IDeleteOrderBillsOperationService deleteBillsService,
                              ICreateOrderBillsOperationService createOrderBillsService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _publicService = publicService;
            _deleteBillsService = deleteBillsService;
            _createOrderBillsService = createOrderBillsService;
        }

        public ActionResult DeleteAll()
        {
            return View(new DeleteAllBillsViewModel());
        }

        [HttpPost]
        public ActionResult DeleteAll(DeleteAllBillsViewModel viewModel)
        {
            try
            {
                _deleteBillsService.Delete(viewModel.OrderId);
                viewModel.Message = BLResources.OK;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                viewModel.SetCriticalError(ex.Message);
                return View(viewModel);
            }
        }

        [HttpPost]
        public JsonNetResult GetRelatedOrdersInfoForCreateBill(long orderId)
        {
            var response = (GetRelatedOrdersForCreateBillResponse)_publicService.Handle(new GetRelatedOrdersForCreateBillRequest { OrderId = orderId });
            return new JsonNetResult(response.Orders);
        }

        public JsonNetResult GetInitPaymentsInfo(long? orderId, BillPaymentType paymentType, int? paymentAmount)
        {
            var response = (GetInitPaymentsInfoResponse)
                           _publicService.Handle(new GetInitPaymentsInfoRequest
                           {
                               OrderId = orderId,
                               PaymentType = paymentType,
                               PaymentAmount = paymentAmount,
                               PaymentDatePlanEvaluator = PaymentDatePlanEvaluator
                           });
            return new JsonNetResult(response.PaymentsInfo);
        }

        [HttpPost]
        public JsonNetResult GetDistributedPaymentsInfo(long? orderId, string payments)
        {
            var initPaymentsInfo = JsonConvert.DeserializeObject<DistributedPaymentsInfo[]>(payments);
            var response = (GetDistributedPaymentsInfoResponse)
                           _publicService.Handle(new GetDistributedPaymentsInfoRequest
                           {
                               OrderId = orderId,
                               PaymentDatePlanEvaluator = PaymentDatePlanEvaluator,
                               InitPaymentsInfos = initPaymentsInfo
                           });

            return new JsonNetResult(response.DistributedPaymentsInfos);
        }

        [HttpPost]
        public void SavePayments(long orderId, string paymentsInfo, string relatedOrders)
        {
            var createBillInfos = JsonConvert.DeserializeObject<CreateBillInfo[]>(paymentsInfo);
            var orders = JsonConvert.DeserializeObject<long[]>(relatedOrders);

            _createOrderBillsService.Create(orderId, createBillInfos);

            if (orders != null && orders.Length > 0)
            {
                var response = (CreateBillInfoForOrdersByTemplateResponse)
                               _publicService.Handle(new CreateBillInfoForOrdersByTemplateRequest
                               {
                                   CreateBillInfosTemplate = createBillInfos,
                                   OrderIds = orders
                               });
                foreach (var orderInfo in response.OrdersCreateBillInfos)
                {
                    _createOrderBillsService.Create(orderInfo.Item1, orderInfo.Item2);
                }
            }
        }
    }
}
