using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.MultiCulture.Controllers
{
    public sealed class CreateBillController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public CreateBillController(IMsCrmSettings msCrmSettings,
                                    IAPIOperationsServiceSettings operationsServiceSettings,
                                    IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                    IIdentityServiceClientSettings identityServiceSettings,
                                    IUserContext userContext,
                                    ITracer tracer,
                                    IGetBaseCurrencyService getBaseCurrencyService,
                                    IPublicService publicService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _publicService = publicService;
        }

        [UseDependencyFields]
        public ActionResult Create(long orderId)
        {
            var response = (GetRelatedOrdersForCreateBillResponse)_publicService.Handle(new GetRelatedOrdersForCreateBillRequest { OrderId = orderId });
            var model = new MultiCultureCreateBillViewModel { OrderId = orderId };
            model.IsMassBillCreateAvailable = response.Orders != null && response.Orders.Length > 0;
            return View(model);
        }
    }
}
