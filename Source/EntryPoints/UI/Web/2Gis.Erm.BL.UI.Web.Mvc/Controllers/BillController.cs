using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

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
        private readonly IOrderReadModel _orderReadModel;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly ISecureFinder _finder;

        public BillController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IPublicService publicService,
                              IOrderReadModel orderReadModel,
                              ILegalPersonRepository legalPersonRepository,
                              ISecureFinder finder)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
            _publicService = publicService;
            _orderReadModel = orderReadModel;
            _legalPersonRepository = legalPersonRepository;
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

        [HttpPost]
        public JsonNetResult GetRelatedOrdersInfoForPrintJointBill(long id)
        {
            var billInfo = _finder.Find<Bill>(b => b.Id == id && b.IsActive && !b.IsDeleted).FirstOrDefault();
            if (billInfo == null)
            {
                return new JsonNetResult(null);
            }

            var response = (GetRelatedOrdersForPrintJointBillResponse)_publicService.Handle(new GetRelatedOrdersForPrintJointBillRequest { OrderId = billInfo.OrderId });
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

        [HttpGet]
        public ActionResult PrintBill(long id, long? profileId)
        {
            try
            {
                var response =
                    (StreamResponse)
                    _publicService.Handle(new PrintBillRequest { Id = id, LegalPersonProfileId = profileId });
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }
        }

        public JsonNetResult IsChooseProfileNeeded(long billId)
        {
            bool isChooseProfileNeeded = true;
            long? legalPersonProfile = null;

            var order = _orderReadModel.GetOrderByBill(billId);
            if (order != null && order.LegalPersonId.HasValue)
            {
                var legalPersonWithProfiles =
                    _legalPersonRepository.GetLegalPersonWithProfiles(order.LegalPersonId.Value);
                if (legalPersonWithProfiles.Profiles.Count() == 1)
                {
                    isChooseProfileNeeded = false;
                    legalPersonProfile = legalPersonWithProfiles.Profiles.First().Id;
                }
            }

            return new JsonNetResult(new
            {
                IsChooseProfileNeeded = isChooseProfileNeeded,
                LegalPersonProfileId = legalPersonProfile
            });
        }

        public ActionResult Print(long id)
        {
            var order = _orderReadModel.GetOrderByBill(id);
            if (order == null || !order.LegalPersonId.HasValue)
            {
                throw new ArgumentException("LegalPersonId");
            }

            var printOrderModel = new PrintOrderViewModel
            {
                LegalPersonId = order.LegalPersonId.Value,
                OrderId = id
            };

            return View(printOrderModel);
        }
    }
}
