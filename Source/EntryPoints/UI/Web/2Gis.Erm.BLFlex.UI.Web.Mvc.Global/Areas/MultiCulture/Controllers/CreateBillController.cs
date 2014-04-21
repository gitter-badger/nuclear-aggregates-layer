﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.MultiCulture.Controllers
{
    public sealed class CreateBillController : ControllerBase
    {
        private readonly IPublicService _publicService;

        public CreateBillController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService,
                              IPublicService publicService)
            : base(msCrmSettings, userContext, logger, operationsServiceSettings, getBaseCurrencyService)
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
