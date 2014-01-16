using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    public sealed class BargainController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IBargainService _bargainService;

        public BargainController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IBargainService bargainService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _bargainService = bargainService;
        }

        [HttpPost]
        public JsonNetResult CreateBargainForOrder(long orderId)
        {
            var bargainInfo = _bargainService.CreateBargainForOrder(orderId);
            return new JsonNetResult(new { BargainId = bargainInfo.Id, BargainNumber = bargainInfo.Number });
        }

        [HttpGet]
        public ActionResult CloseBargains()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CloseBargainOperationalPeriod, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.CloseBargains_AccessDenied);
            }

            var model = new CloseBargainsViewModel { CloseDate = DateTime.Now };
            return View(model);
        }

        public ActionResult CloseBargains(CloseBargainsViewModel model)
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CloseBargainOperationalPeriod, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.CloseBargains_AccessDenied);
            }

            if (model.CloseDate.HasValue)
            {
                try
                {
                    CloseBargainsResult result = _bargainService.CloseBargains(model.CloseDate.Value);

                    if (result.NonClosedBargainsNumbers != null && result.NonClosedBargainsNumbers.Length > 0)
                    {
                        model.HasCompleted = false;
                        model.SetInfo(string.Format(BLResources.CloseBargains_BargainsPeriodConstraintNotFullfilled,
                                                    string.Join(", ", result.NonClosedBargainsNumbers)));
                    }
                    else
                    {
                        model.HasCompleted = true;
                        model.SetInfo(BLResources.CloseBargains_Completed);
                    }
                }
                catch (Exception ex)
                {
                    model.SetCriticalError(ex.Message);
                    model.HasCompleted = false;
                }
            }
            else
            {
                model.SetCriticalError(BLResources.CloseBargains_DateNotSpecified);
                model.HasCompleted = false;
            }

            return View(model);
        }

        [HttpPost]
        public JsonNetResult GetBargain(long? branchOfficeOrganizationUnitId, long? legalPersonId, DateTime orderSignupDate)
        {
            if (!branchOfficeOrganizationUnitId.HasValue || !legalPersonId.HasValue)
            {
                return new JsonNetResult();
            }

            var resp = _bargainService.GetBargain(branchOfficeOrganizationUnitId, legalPersonId, orderSignupDate);
            return new JsonNetResult((resp.BargainId != default(long)) ? new { Id = resp.BargainId, resp.BargainNumber, resp.BargainClosedOn } : null);
        }
    }
}
