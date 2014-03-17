using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Chile.Controllers
{
    public sealed class CreateBillController : ControllerBase
    {
        public CreateBillController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
        {
        }

        [UseDependencyFields]
        public ActionResult Create(long orderId)
        {
            var model = new ChileCreateBillViewModel { OrderId = orderId };
            return View(model);
        }
    }
}
