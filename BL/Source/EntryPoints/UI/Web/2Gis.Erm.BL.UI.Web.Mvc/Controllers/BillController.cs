using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class BillController : ControllerBase
    {
        // ��� ������� ������� - 20 ����� ������, ��� ����������� - 10 ����� ������
        // ���� ���� ���������� > 20-�� ����� �������� ������ �� "���� ������, ��" � ������ (������������) ����� ������������� = ���� ���������� ��.
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
        private readonly ISecureFinder _finder;

        public BillController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IPublicService publicService,
                              ISecureFinder finder)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, specialOperationsServiceSettings, getBaseCurrencyService)
        {
            _publicService = publicService;
            _finder = finder;
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
                _publicService.Handle(new DeleteBillsRequest { OrderId = viewModel.OrderId });
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

            _publicService.Handle(new CreateBillsRequest
            {
                OrderId = orderId,
                CreateBillInfos = createBillInfos,
            });

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
                    _publicService.Handle(new CreateBillsRequest
                    {
                        OrderId = orderInfo.Item1,
                        CreateBillInfos = orderInfo.Item2,
                    });
                }
            }
        }
    }
}
