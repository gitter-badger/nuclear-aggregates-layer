using System.Net;
using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    // handling non-500 errors throwed by asp.net runtime
    public sealed class ErrorController : ControllerBase
    {
        public ErrorController(IMsCrmSettings msCrmSettings,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IIdentityServiceClientSettings identityServiceSettings,
                               IUserContext userContext,
                               ITracer tracer,
                               IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
        }

        // custom 404 error
        public ActionResult PageNotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            var model = new ErrorHandlerModel
                            {
                                Title = BLResources.Error404,
                                Text = BLResources.UrlDoesNotExists,
                            };

            return View("Error", model);
        }

        // custom 401 error (actually return status 200)
        public ActionResult NonAuthenticated()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;

            var model = new ErrorHandlerModel
                            {
                                Title = BLResources.NonAuthenticated,
                                Text = string.Format(BLResources.UnrecognizedUser, User.Identity.Name),
                            };

            return View("Error", model);
        }

        // custom 401 error (actually return status 200)
        public ActionResult IncompatibleDBVersion()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var model = new ErrorHandlerModel
                            {
                                Title = BLResources.Error,
                                Text = BLResources.IncorrectDBVersion,
                            };

            return View("Error", model);
        }

        public ActionResult UnderConstruction()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;

            var model = new ErrorHandlerModel
                            {
                                Title = BLResources.Error,
                                Text = BLResources.UnderConstruction,
                            };

            return View("Error", model);
        }

        [HttpPost]
        public JsonNetResult LogError()
        {
            Tracer.WarnFormat("Javascript exception occured: {0}", HttpUtility.UrlDecode(Request.Params.ToString()));
            return new JsonNetResult();
        }
    }
}