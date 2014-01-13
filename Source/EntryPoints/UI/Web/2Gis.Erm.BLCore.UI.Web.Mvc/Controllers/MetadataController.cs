using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class MetadataController : ControllerBase
    {
        private readonly IAPIIntrospectionServiceSettings _introspectionServiceSettings;

        public MetadataController(
            IMsCrmSettings msCrmSettings, 
            IUserContext userContext, 
            ICommonLog logger, 
            IAPIOperationsServiceSettings operationsServiceSettings,
            IAPIIntrospectionServiceSettings introspectionServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
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