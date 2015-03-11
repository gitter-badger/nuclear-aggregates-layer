using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Aggregates.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AdvertisementTemplates;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdvertisementTemplateController : ControllerBase
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IPublicService _publicService;

        public AdvertisementTemplateController(IMsCrmSettings msCrmSettings,
                                               IAPIOperationsServiceSettings operationsServiceSettings,
                                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                               IAPIIdentityServiceSettings identityServiceSettings,
                                               IUserContext userContext,
                                               ITracer tracer,
                                               IGetBaseCurrencyService getBaseCurrencyService,
                                               IAdvertisementRepository advertisementRepository,
                                               IPublicService publicService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
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

        [HttpPost]
        public ActionResult Publish(long advertisementTemplateId)
        {
            var publishAdvertisementTemplateRequest = new PublishAdvertisementTemplateRequest
                                                          {
                                                              AdvertisementTemplateId = advertisementTemplateId
                                                          };

            _publicService.Handle(publishAdvertisementTemplateRequest);

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Unpublish(long advertisementTemplateId)
        {
            var unpublishAdvertisementTemplateRequest = new UnpublishAdvertisementTemplateRequest
                                                            {
                                                                AdvertisementTemplateId = advertisementTemplateId
                                                            };

            _publicService.Handle(unpublishAdvertisementTemplateRequest);

            return new EmptyResult();
        }
    }
}