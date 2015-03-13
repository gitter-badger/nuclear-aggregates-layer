using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bargains;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class BargainController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ICloseClientBargainsOperationService _closeClientBargainsOperationService;
        private readonly IOrderReadModel _orderReadModel;

        public BargainController(IMsCrmSettings msCrmSettings,
                                 IAPIOperationsServiceSettings operationsServiceSettings,
                                 IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                 IIdentityServiceClientSettings identityServiceSettings,
                                 IUserContext userContext,
                                 ITracer tracer,
                                 IGetBaseCurrencyService getBaseCurrencyService,
                                 ISecurityServiceFunctionalAccess functionalAccessService,
                                 ICloseClientBargainsOperationService closeClientBargainsOperationService,
                                 IOrderReadModel orderReadModel)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _functionalAccessService = functionalAccessService;
            _closeClientBargainsOperationService = closeClientBargainsOperationService;
            _orderReadModel = orderReadModel;
        }

        [HttpGet]
        public ActionResult CloseBargains()
        {
            if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CloseBargainOperationalPeriod, UserContext.Identity.Code))
            {
                throw new NotificationException(BLResources.CloseBargains_AccessDenied);
            }

            var model = new CloseBargainsViewModel { CloseDate = DateTime.UtcNow };
            return View(model);
        }

        [HttpGet]
        public ViewResult SelectLegalPersonProfile(long bargainId)
        {
            var dto = _orderReadModel.GetLegalPersonProfileByBargain(bargainId);

            var model = new SelectLegalPersonProfileViewModel
                            {
                                LegalPerson = dto.LegalPerson.ToLookupField(),
                                LegalPersonProfile = dto.LegalPersonProfile.ToLookupField()
                            };

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
                    var result = _closeClientBargainsOperationService.CloseClientBargains(model.CloseDate.Value.Date);

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
    }
}
