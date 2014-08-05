using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
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
            IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
            IAPIIntrospectionServiceSettings introspectionServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, specialOperationsServiceSettings, getBaseCurrencyService)
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