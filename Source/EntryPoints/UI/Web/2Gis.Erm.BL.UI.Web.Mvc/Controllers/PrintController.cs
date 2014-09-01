using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class PrintController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _secureFinder;

        public PrintController(IMsCrmSettings msCrmSettings,
                               IUserContext userContext,
                               ICommonLog logger,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               IPublicService publicService,
                               ISecureFinder secureFinder)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _publicService = publicService;
            _secureFinder = secureFinder;
        }

        [HttpGet]
        public ActionResult PrepareJointBill(long id)
        {
            var model = new PrepareJointBillViewModel
            {
                EntityId = id,
                EntityName = typeof(Order).AsEntityName(),
                IsMassBillCreateAvailable = false
            };

            var orderInfo = _secureFinder.Find(Specs.Find.ById<Order>(id) & Specs.Find.ActiveAndNotDeleted<Order>()).FirstOrDefault();
            if (orderInfo != null)
            {
                var request = new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id };
                var response = (GetRelatedOrdersForPrintJointBillResponse)_publicService.Handle(request);
                model.IsMassBillCreateAvailable = response.Orders != null && response.Orders.Length > 0;
            }

            return View(model);
        }

        [HttpGet]
        public JsonNetResult GetRelatedOrdersInfoForPrintJointBill(long orderId)
        {
            var orderInfo = _secureFinder.Find<Order>(o => o.Id == orderId && o.IsActive && !o.IsDeleted).FirstOrDefault();
            if (orderInfo == null)
            {
                return new JsonNetResult(null);
            }

            var request = new GetRelatedOrdersForPrintJointBillRequest { OrderId = orderInfo.Id };
            var response = (GetRelatedOrdersForPrintJointBillResponse)_publicService.Handle(request);
            return new JsonNetResult(response.Orders);
        }

        [HttpPost]
        public ActionResult PrintJointBill(long id, string relatedOrders)
        {
            var orders = JsonConvert.DeserializeObject<long[]>(relatedOrders);

            if (orders != null && orders.Length > 0)
            {
                return TryPrintDocument(new PrintOrderJointBillRequest { OrderId = id, RelatedOrderIds = orders });
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult PrintOrder(long id)
        {
            return TryPrintDocument(new PrintOrderWithGuarateeRequest { OrderId = id });
        }
        
        [HttpGet]
        public ActionResult PrintSingleBill(long id)
        {
            return TryPrintDocument(new PrintBillRequest { BillId = id });
        }

        [HttpGet]
        public ActionResult PrintReferenceInformation(long id)
        {
            return TryPrintDocument(new PrintReferenceInformationRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintRegionalOrder(long id)
        {
            return TryPrintDocument(new PrintRegionalOrderRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintBargain(long id)
        {
            return TryPrintDocument(new PrintOrderBargainRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintNewSalesModelBargain(long id)
        {
            return TryPrintDocument(new PrintNewSalesModelBargainRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintOrderBills(long id)
        {
            return TryPrintDocument(new PrintOrderBillsRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintLetterOfGuarantee(long id)
        {
            return TryPrintDocument(new PrintLetterOfGuaranteeRequest { OrderId = id, IsChangingAdvMaterial = true });
        }

        [HttpGet]
        public ActionResult PrintTerminationNotice(long id)
        {
            return TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintTerminationNoticeWithoutReason(long id)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, WithoutReason = true });
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNotice(long id)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest { OrderId = id, TerminationBargain = true });
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNoticeWithoutReason(long id)
        {
            return
                TryPrintDocument(new PrintOrderTerminationNoticeRequest
                                     {
                                         OrderId = id,
                                         WithoutReason = true,
                                         TerminationBargain = true
                                     });
        }

        [HttpGet]
        public ActionResult PrintRegionalTerminationNotice(long id)
        {
            return
                TryPrintDocument(new PrintRegionalOrderTerminationNoticeRequest { OrderId = id });
        }

        [HttpGet]
        public ActionResult PrintAdditionalAgreement(long id)
        {
            return
                TryPrintDocument(new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         PrintType = PrintAdditionalAgreementTarget.Order
                                     });
        }

        [HttpGet]
        public ActionResult PrintBargainAdditionalAgreement(long id)
        {
            return
                TryPrintDocument(new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         PrintType = PrintAdditionalAgreementTarget.Bargain
                                     });
        }

        private ActionResult TryPrintDocument(Request printRequest)
        {
            try
            {
                var response = (StreamResponse)_publicService.Handle(printRequest);
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (BusinessLogicException exception)
            {
                return new ContentResult { ContentType = MediaTypeNames.Text.Plain, Content = exception.Message };
            }
        }
    }
}
