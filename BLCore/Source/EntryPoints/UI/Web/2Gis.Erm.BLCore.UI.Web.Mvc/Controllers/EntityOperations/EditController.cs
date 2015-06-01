using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using NuClear.Model.Common.Entities;
using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

// namespace DoubleGis.Erm.Web.Controllers.EntityOperations

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations
{
    public class EditController : ControllerBase
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        public EditController(IMsCrmSettings msCrmSettings,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IIdentityServiceClientSettings identityServiceSettings,
                              IUserContext userContext,
                              ITracer tracer,
                              IGetBaseCurrencyService getBaseCurrencyService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
        }

        [HttpGet]
        public ActionResult EntityPrivileges(IEntityType entityTypeName, long? entityId, string entityState)
        {
            if (!entityId.HasValue)
            {
                throw new NotificationException(BLResources.IdentifierNotSet);
            }

            return View(new EditPrivilegeViewModel { RoleId = entityId.Value });
        }

        [HttpGet]
        public ActionResult FunctionalPrivileges(IEntityType entityTypeName, long? entityId, string entityState)
        {
            if (!entityId.HasValue)
            {
                throw new NotificationException(BLResources.IdentifierNotSet);
            }

            return View(new EditPrivilegeViewModel { RoleId = entityId.Value });
        }
    }
}