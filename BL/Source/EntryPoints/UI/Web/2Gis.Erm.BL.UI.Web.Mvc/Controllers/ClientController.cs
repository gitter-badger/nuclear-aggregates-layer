using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class ClientController : ControllerBase
    {
        public ClientController(IMsCrmSettings msCrmSettings,
                                IAPIOperationsServiceSettings operationsServiceSettings,
                                IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                IAPIIdentityServiceSettings identityServiceSettings,
                                IUserContext userContext,
                                ICommonLog logger,
                                IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
        }

        public ActionResult AllClientsDebtReport()
        {
            return View(new AllClientsDebtReportViewModel
                            {
                                Owner = new LookupField { Key = UserContext.Identity.Code, Value = UserContext.Identity.DisplayName }
                            });
        }
    }
}