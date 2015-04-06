using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class MetadataController : ControllerBase
    {
        private readonly IAPIIntrospectionServiceSettings _introspectionServiceSettings;

        public MetadataController(IMsCrmSettings msCrmSettings,
                                  IAPIOperationsServiceSettings operationsServiceSettings,
                                  IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                  IAPIIdentityServiceSettings identityServiceSettings,
                                  IUserContext userContext,
                                  ITracer tracer,
                                  IGetBaseCurrencyService getBaseCurrencyService,
                                  IAPIIntrospectionServiceSettings introspectionServiceSettings)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _introspectionServiceSettings = introspectionServiceSettings;
        }

        public ActionResult Services()
        {
            var model = new ServicesViewModel()
                {
                    IntrospectionServiceAddress = _introspectionServiceSettings.RestUrl.ToString(),
                };

            return View("Services", model);
        }
    }
}