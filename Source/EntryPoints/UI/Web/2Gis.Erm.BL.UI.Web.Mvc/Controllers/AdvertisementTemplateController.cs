using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdvertisementTemplateController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IPublicService _publicService;

        public AdvertisementTemplateController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IAdvertisementRepository advertisementRepository,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IPublicService publicService,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   getBaseCurrencyService)
        {
            _advertisementRepository = advertisementRepository;
            _publicService = publicService;
        }

        [HttpGet]
        public JsonNetResult GetAdvertisementTemplate(long advertisementTemplateId)
        {
            var result = _advertisementRepository.GetAdvertisementTemplate(advertisementTemplateId);
            return new JsonNetResult(result);
        }

        [HttpGet]
        public ActionResult Publish()
        {
            return View(new PublishAdvertisementTemplateModel());
        }

        [HttpPost]
        public ActionResult Publish(PublishAdvertisementTemplateModel model)
        {
            try
            {
                var publishAdvertisementTemplateRequest = new PublishAdvertisementTemplateRequest
                    {
                        AdvertisementTemplateId = model.Id
                    };
                _publicService.Handle(publishAdvertisementTemplateRequest);

                model.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                model.SetCriticalError(ex.Message);
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Unpublish()
        {
            return View(new UnpublishAdvertisementTemplateModel());
        }

        [HttpPost]
        public ActionResult Unpublish(UnpublishAdvertisementTemplateModel model)
        {
            try
            {
                var unpublishAdvertisementTemplateRequest = new UnpublishAdvertisementTemplateRequest
                    {
                        AdvertisementTemplateId = model.Id
                    };
                _publicService.Handle(unpublishAdvertisementTemplateRequest);

                model.Message = BLResources.OK;
            }
            catch (Exception ex)
            {
                model.SetCriticalError(ex.Message);
                return View(model);
            }

            return View(model);
        }
    }
}
