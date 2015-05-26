using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Common.Metadata.Old;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class MainController : ControllerBase
    {
        private readonly IUIConfigurationService _configurationService;

        public MainController(IMsCrmSettings msCrmSettings,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IIdentityServiceClientSettings identityServiceSettings,
                              IUserContext userContext,
                              ITracer tracer,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IUIConfigurationService configurationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _configurationService = configurationService;
        }

        public ActionResult Index()
        {
            return View(new MainPageViewModel { Settings = new { Items = _configurationService.GetNavigationSettings(UserContext.Profile.UserLocaleInfo.UserCultureInfo) } });
        }
    }
}