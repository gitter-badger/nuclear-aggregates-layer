using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using NuClear.Security.API.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Chile.Controllers
{
    public sealed class CreateBillController : ControllerBase
    {
        public CreateBillController(IMsCrmSettings msCrmSettings,
                                    IAPIOperationsServiceSettings operationsServiceSettings,
                                    IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                    IAPIIdentityServiceSettings identityServiceSettings,
                                    IUserContext userContext,
                                    ITracer tracer,
                                    IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
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