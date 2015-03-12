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
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using Newtonsoft.Json;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class PrintController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly ISecureFinder _secureFinder;

        public PrintController(IMsCrmSettings msCrmSettings,
                               IUserContext userContext,
                               ITracer tracer,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               IPublicService publicService,
                               ISecureFinder secureFinder,
                               IIdentityServiceClientSettings identityServiceSettings)
            : base(msCrmSettings,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   identityServiceSettings,
                   userContext,
                   tracer,
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
            var request = new PrintOrderWithGuarateeRequest { OrderId = id };
            return TryPrintDocument(request);
        }
        
        [HttpGet]
        public ActionResult PrintSingleBill(long id)
        {
            var request = new PrintBillRequest { BillId = id };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintBargainProlongationAgreement(long id, long profileId)
        {
            var request = new PrintBargainProlongationAgreementRequest { BargainId = id, LegalPersonProfileId = profileId };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintBargain(long id, long profileId)
        {
            var request = new PrintOrderBargainRequest { BargainId = id, LegalPersonProfileId = profileId };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintNewSalesModelBargain(long id, long profileId)
        {
            var request = new PrintNewSalesModelBargainRequest { BargainId = id, LegalPersonProfileId = profileId };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintOrderBargain(long id)
        {
            var request = new PrintOrderBargainRequest { OrderId = id };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintNewSalesModelOrderBargain(long id)
        {
            var request = new PrintNewSalesModelBargainRequest { OrderId = id };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintOrderBills(long id)
        {
            var request = new PrintOrderBillsRequest { OrderId = id };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintLetterOfGuarantee(long id)
        {
            var request = new PrintLetterOfGuaranteeRequest { OrderId = id, IsChangingAdvMaterial = true };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintTerminationNotice(long id)
        {
            var request = new PrintOrderTerminationNoticeRequest { OrderId = id };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintTerminationNoticeWithoutReason(long id)
        {
            var request = new PrintOrderTerminationNoticeRequest { OrderId = id, WithoutReason = true };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNotice(long id)
        {
            var request = new PrintOrderTerminationNoticeRequest { OrderId = id, TerminationBargain = true };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNoticeWithoutReason(long id)
        {
            var request = new PrintOrderTerminationNoticeRequest
                                     {
                                         OrderId = id,
                                         WithoutReason = true,
                                         TerminationBargain = true
                              };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintAdditionalAgreement(long id)
        {
            var request = new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         PrintType = PrintAdditionalAgreementTarget.Order
                              };
            return TryPrintDocument(request);
        }

        [HttpGet]
        public ActionResult PrintBargainAdditionalAgreement(long id)
        {
            var request = new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         PrintType = PrintAdditionalAgreementTarget.Bargain
                              };
            return TryPrintDocument(request);
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
