using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.Administration;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public sealed class AdministrationController : ControllerBase
    {
        private readonly ISecurityServiceFunctionalAccess _securityServiceFunctionalAccess;

        public AdministrationController(IMsCrmSettings msCrmSettings,
                                        IAPIOperationsServiceSettings operationsServiceSettings,
                                        IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                        IAPIIdentityServiceSettings identityServiceSettings,
                                        IUserContext userContext,
                                        ICommonLog logger,
                                        IGetBaseCurrencyService getBaseCurrencyService,
                                        ISecurityServiceFunctionalAccess securityServiceFunctionalAccess)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _securityServiceFunctionalAccess = securityServiceFunctionalAccess;
        }

        public ActionResult Administration()
        {
            return View();
        }

        public ActionResult CatalogueManagement()
        {
            return View();
        }

        public ActionResult SecurityAdministration()
        {
            return View();
        }

        public ActionResult MessageQueueManagement()
        {
            var model = new MessageQueueManagementViewModel
                            {
                                HasAccessToCorporateQueue = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.CorporateQueueAccess, UserContext.Identity.Code),
                                HasAccessToRelease = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ReleaseAccess, UserContext.Identity.Code),
                                HasAccessToWithdrawal = _securityServiceFunctionalAccess.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.WithdrawalAccess, UserContext.Identity.Code)
                            };

            return View(model);
        }
    }
}
