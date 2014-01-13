using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Themes;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers
{
    // TODO {all, 26.11.2013}: вынести в сервис API.Operations, контроллер удалить
    public sealed class ThemeController : ControllerBase
    {
        private readonly ISetAsDefaultThemeOperationService _setAsDefaultThemeOperationService;

        public ThemeController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext, 
            ICommonLog logger,
            ISetAsDefaultThemeOperationService setAsDefaultThemeThemeOperationServiceService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService)
        : base(
            msCrmSettings,
            userContext,
            logger,
            operationsServiceSettings,
            getBaseCurrencyService)
        {
            _setAsDefaultThemeOperationService = setAsDefaultThemeThemeOperationServiceService;
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
