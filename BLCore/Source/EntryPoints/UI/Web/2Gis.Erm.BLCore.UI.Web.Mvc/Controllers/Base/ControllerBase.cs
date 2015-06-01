using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base
{
    public abstract class ControllerBase : Controller
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IAPIOperationsServiceSettings _operationsServiceSettings;
        private readonly IAPISpecialOperationsServiceSettings _specialOperationsServiceSettings;
        private readonly IIdentityServiceClientSettings _identityServiceSettings;
        private readonly IUserContext _userContext;
        private readonly ITracer _tracer;
        private readonly IGetBaseCurrencyService _getBaseCurrencyService;

        protected ControllerBase(IMsCrmSettings msCrmSettings,
                                 IAPIOperationsServiceSettings operationsServiceSettings,
                                 IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                 IIdentityServiceClientSettings identityServiceSettings,
                                 IUserContext userContext,
                                 ITracer tracer,
                                 IGetBaseCurrencyService getBaseCurrencyService)
        {
            _msCrmSettings = msCrmSettings;
            _operationsServiceSettings = operationsServiceSettings;
            _specialOperationsServiceSettings = specialOperationsServiceSettings;
            _identityServiceSettings = identityServiceSettings;
            _userContext = userContext;
            _tracer = tracer;
            _getBaseCurrencyService = getBaseCurrencyService;
        }

        protected IUserContext UserContext
        {
            get { return _userContext; }
        }

        protected ITracer Tracer
        {
            get { return _tracer; }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Request.IsAjaxRequest())
            {
                // сюда может попадать поток выполнения и когда пользователь не аутентифицирован
                // Например, когда версия приложения != версии БД - сразу же из BeginRequest вызов перенаправляется на ErrorController
                // Т.е. к моменту запуска ErrorController AquireState даже не отработал - т.е. пользователя даже не пытались аутентифицировать =>:
                // usercontext не может содержать корректные данные об identity пользователя и о его профиле
                ViewData[IdentityWebExtensions.UserIdentityInfoKey] = _userContext.Identity.UserData;
                ViewData[UserLocaleInfo.UserLocaleInfoKey] = _userContext.Profile.ToUserLocalInfo();
                ViewData[WebAppSettingsExtension.MsCrmSettingsKey] = _msCrmSettings;
                ViewData[WebAppSettingsExtension.BasicOperationsServiceRestUrlKey] = _operationsServiceSettings.RestUrl.ToString();
                ViewData[WebAppSettingsExtension.SpecialOperationsServiceRestUrlKey] = _specialOperationsServiceSettings.RestUrl.ToString();
                ViewData[WebAppSettingsExtension.IdentityServiceRestUrlKey] = _identityServiceSettings.IdentityServiceUrl.ToString();
                ViewData[WebAppSettingsExtension.ErmBaseCurrencyKey] = _getBaseCurrencyService.GetBaseCurrency().Symbol;
            }
        }

        protected override void ExecuteCore()
        {
            // log controller action
            var controllerName = RouteData.GetRequiredString("controller");
            var actionName = RouteData.GetRequiredString("action");
            Tracer.DebugFormat("Вызов контроллера [{0}]. Метод [{1}]", controllerName, actionName);

            base.ExecuteCore();
        }
    }
}
