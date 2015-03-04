using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Themes;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;

using Nuclear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    // TODO {all, 26.11.2013}: вынести в сервис API.Operations, контроллер удалить
    public sealed class ThemeController : ControllerBase
    {
        private readonly ISetAsDefaultThemeOperationService _setAsDefaultThemeOperationService;

        public ThemeController(IMsCrmSettings msCrmSettings,
                               IAPIOperationsServiceSettings operationsServiceSettings,
                               IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                               IAPIIdentityServiceSettings identityServiceSettings,
                               IUserContext userContext,
                               ITracer logger,
                               IGetBaseCurrencyService getBaseCurrencyService,
                               ISetAsDefaultThemeOperationService setAsDefaultThemeOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
            _setAsDefaultThemeOperationService = setAsDefaultThemeOperationService;
        }

        [HttpPost]
        public ActionResult SetAsDefault(long id, bool isDefault)
        {
            try
            {
                _setAsDefaultThemeOperationService.SetAsDefault(id, isDefault);
            }
            catch (ArgumentException e)
            {
                throw new NotificationException(e.Message, e);
            }
            
            return new EmptyResult();
        }
    }
}
