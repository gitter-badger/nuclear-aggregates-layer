using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
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
        private readonly IOrderReadModel _orderReadModel;
        private readonly ProfileChooseHelper _profileChooseHelper;

        public PrintController(IMsCrmSettings msCrmSettings,
                               IUserContext userContext,
                               ICommonLog logger,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               IPublicService publicService,
                               ISecureFinder secureFinder,
                               IOrderReadModel orderReadModel,
                               ILegalPersonReadModel legalPersonReadModel,
                               ISecurityServiceEntityAccess securityServiceEntityAccess)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _publicService = publicService;
            _secureFinder = secureFinder;
            _orderReadModel = orderReadModel;
            _profileChooseHelper = new ProfileChooseHelper(orderReadModel, legalPersonReadModel, securityServiceEntityAccess);
        }

        [HttpGet]
        public ActionResult IsChooseProfileNeeded(long? orderId, long? billId, long? bargainId)
        {
            ProfileChooseHelper.ChooseProfileDialogState chooseProfileDialogState;
            if (orderId.HasValue)
            {
                chooseProfileDialogState = _profileChooseHelper.GetChooseProfileDialogStateForOrder(orderId.Value);
            }
            else if (billId.HasValue)
            {
                chooseProfileDialogState = _profileChooseHelper.GetChooseProfileDialogStateForBill(billId.Value);
            }
            else if (bargainId.HasValue)
            {
                chooseProfileDialogState = _profileChooseHelper.GetChooseProfileDialogStateForBargain(bargainId.Value);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Either 'orderId' or 'billId' or 'bargainId' must be specified");
            }

            return new JsonNetResult(new
            {
                IsChooseProfileNeeded = chooseProfileDialogState.IsChooseProfileNeeded,
                LegalPersonProfileId = chooseProfileDialogState.LegalPersonProfileId
            });
        }

        [HttpGet]
        public ActionResult ChooseProfile(long? orderId, long? billId, long? bargainId, long? profileId)
        {
            ChooseProfileViewModel viewModel;
            if (orderId.HasValue)
            {
                viewModel = _profileChooseHelper.GetViewModelByOrder(orderId.Value, UserContext.Identity.Code, profileId);
            }
            else if (billId.HasValue)
            {
                viewModel = _profileChooseHelper.GetViewModelByBill(billId.Value, UserContext.Identity.Code, profileId);
            }
            else if (bargainId.HasValue)
            {
                viewModel = _profileChooseHelper.GetViewModelByBargain(bargainId.Value, profileId);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Either 'orderId' or 'billId' or 'bargainId' must be specified");
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult PrepareJointBill(long id, long profileId)
        {
            var model = new PrepareJointBillViewModel
            {
                EntityId = id,
                EntityName = typeof(Order).AsEntityName(),
                ProfileId = profileId,
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
        public ActionResult PrintJointBill(long id, string relatedOrders, long profileId)
        {
            var orders = JsonConvert.DeserializeObject<long[]>(relatedOrders);

            if (orders != null && orders.Length > 0)
            {
                return TryPrintDocument(id,
                                        profileId,
                                        new PrintOrderJointBillRequest { OrderId = id, RelatedOrderIds = orders, LegalPersonProfile = profileId },
                                        true);
            }

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult PrintOrder(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintOrderWithGuarateeRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }
        
        [HttpGet]
        public ActionResult PrintSingleBill(long id, long profileId)
        {
            // TODO {all, 28.05.2014}: в будущем нужно будет это отсюда убрать. В специализированный сервис печати, например
            var order = _orderReadModel.GetOrderByBill(id);

            return
                TryPrintDocument(order.Id, profileId, new PrintBillRequest { Id = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintBargainProlongationAgreement(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintBargainProlongationAgreementRequest { BargainId = id, LegalPersonProfileId = profileId }, false);
        }

        [HttpGet]
        public ActionResult PrintBargain(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintOrderBargainRequest { BargainId = id, LegalPersonProfileId = profileId }, false);
        }

        [HttpGet]
        public ActionResult PrintNewSalesModelBargain(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintNewSalesModelBargainRequest { BargainId = id, LegalPersonProfileId = profileId }, false);
        }

        [HttpGet]
        public ActionResult PrintOrderBargain(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintOrderBargainRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintNewSalesModelOrderBargain(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintNewSalesModelBargainRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintOrderBills(long id, long profileId)
        {
            return TryPrintDocument(id, profileId, new PrintOrderBillsRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintLetterOfGuarantee(long id, long profileId)
        {
            return TryPrintDocument(id,
                                    profileId,
                                    new PrintLetterOfGuaranteeRequest { OrderId = id, LegalPersonProfileId = profileId, IsChangingAdvMaterial = true },
                                    true);
        }

        [HttpGet]
        public ActionResult PrintTerminationNotice(long id, long profileId)
        {
            return
                TryPrintDocument(id, profileId, new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintTerminationNoticeWithoutReason(long id, long profileId)
        {
            return
                TryPrintDocument(id,
                                 profileId,
                                 new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId, WithoutReason = true },
                                 true);
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNotice(long id, long profileId)
        {
            return
                TryPrintDocument(id,
                                 profileId,
                                 new PrintOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId, TerminationBargain = true },
                                 true);
        }

        [HttpGet]
        public ActionResult PrintTerminationBargainNoticeWithoutReason(long id, long profileId)
        {
            return
                TryPrintDocument(id,
                                 profileId,
                                 new PrintOrderTerminationNoticeRequest
                                     {
                                         OrderId = id,
                                         LegalPersonProfileId = profileId,
                                         WithoutReason = true,
                                         TerminationBargain = true
                                     },
                                 true);
        }

        [HttpGet]
        public ActionResult PrintRegionalTerminationNotice(long id, long profileId)
        {
            return
                TryPrintDocument(id, profileId, new PrintRegionalOrderTerminationNoticeRequest { OrderId = id, LegalPersonProfileId = profileId }, true);
        }

        [HttpGet]
        public ActionResult PrintAdditionalAgreement(long id, long profileId)
        {
            return
                TryPrintDocument(id,
                                 profileId,
                                 new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         LegalPersonProfileId = profileId,
                                         PrintType = PrintAdditionalAgreementTarget.Order
                                     },
                                 true);
        }

        [HttpGet]
        public ActionResult PrintBargainAdditionalAgreement(long id, long profileId)
        {
            return
                TryPrintDocument(id,
                                 profileId,
                                 new PrintOrderAdditionalAgreementRequest
                                     {
                                         OrderId = id,
                                         LegalPersonProfileId = profileId,
                                         PrintType = PrintAdditionalAgreementTarget.Bargain
                                     },
                                 true);
        }
        
        private ActionResult TryPrintDocument(long id, long profileId, Request printRequest, bool saveChosenProfile)
        {
            try
            {
                // TODO {all, 28.05.2014}: эту логику нужно вынести из контроллера и избавиться от хэндлеров
                if (saveChosenProfile)
                {
                _publicService.Handle(new ChangeOrderLegalPersonProfileRequest { OrderId = id, LegalPersonProfileId = profileId });
                }

                var response = (StreamResponse)_publicService.Handle(printRequest);
                return File(response.Stream, response.ContentType, HttpUtility.UrlPathEncode(response.FileName));
            }
            catch (Exception ex)
            {
                return new ContentResult { Content = ex.Message };
            }
        }
    }
}
