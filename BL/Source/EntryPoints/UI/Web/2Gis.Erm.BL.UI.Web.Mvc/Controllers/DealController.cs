using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class DealController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly IGenerateDealNameService _dealNameService;
        private readonly ISetMainLegalPersonForDealOperationService _setMainLegalPersonForDealOperationService;

        public DealController(IMsCrmSettings msCrmSettings,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IIdentityServiceClientSettings identityServiceSettings,
                              IUserContext userContext,
                              ITracer tracer,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IPublicService publicService,
                              IGenerateDealNameService dealNameService,
                              ISetMainLegalPersonForDealOperationService setMainLegalPersonForDealOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
            _dealNameService = dealNameService;
            _setMainLegalPersonForDealOperationService = setMainLegalPersonForDealOperationService;
        }

        public ActionResult PickCreateReason()
        {
            return View(new DealCreateReasonViewModel());
        }

        #region close

        [HttpGet]
        public ActionResult Close()
        {
            return View(new CloseDealViewModel());
        }

        [HttpPost]
        public JsonNetResult Close(CloseDealViewModel viewModel)
        {
            _publicService.Handle(new CloseDealRequest
            {
                Id = viewModel.Id,
                CloseReason = viewModel.CloseReason,
                CloseReasonOther = viewModel.CloseReasonOther,
                Comment = viewModel.Comment,
            });

            return new JsonNetResult();
        }

        #endregion

        #region reopen deal

        [HttpGet]
        public ActionResult ReopenDeal()
        {
            return View(new ActivateDealViewModel());
        }

        [HttpPost]
        public ActionResult ReopenDeal(ActivateDealViewModel model)
        {
            try
            {
                var response = ((ReopenDealResponse)_publicService.Handle(new ReopenDealRequest { DealId = model.DealId, }));
                var msg = (response.Message == BLResources.OK) ? response.Message : String.Format(BLResources.ErrorDuringOperation, response.Message);
                model.SetInfo(msg);
            }
            catch (Exception ex)
            {
                ModelUtils.OnException(this, Tracer, model, ex);
            }
            return View(model);
        }

        #endregion
        
        [HttpPost]
        public JsonNetResult GenerateDealName(string clientName, string mainFirmName)
        {
            var dealName = _dealNameService.GenerateDealName(clientName, mainFirmName);

            return new JsonNetResult(dealName);
        }

        [HttpPost]
        public JsonNetResult SetMainLegalPerson(long dealId, long legalPersonId)
        {
            _setMainLegalPersonForDealOperationService.SetMainLegalPerson(dealId, legalPersonId);

            return new JsonNetResult();
        }

        public JsonNetResult CheckIsWarmClient(long clientId)
        {
            var request = new CheckIsWarmClientRequest { ClientId = clientId };
            var response = (CheckIsWarmClientResponse)_publicService.Handle(request);
            var result = new JsonNetResult(new
                {
                    IsWarmClient = response.IsWarmClient,
                    TaskActualEnd = response.TaskActualEnd,
                    TaskDescription = response.TaskDescription
                });
            return result;
        }
    }
}
