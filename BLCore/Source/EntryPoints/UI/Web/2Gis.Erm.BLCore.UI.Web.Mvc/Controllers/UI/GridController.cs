using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;

using NuClear.Model.Common.Entities;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

//namespace DoubleGis.Erm.Web.Controllers.UI
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.UI
{
    public class GridController : ControllerBase
    {
        private readonly IUIServicesManager _uiServicesManager;

        public GridController(IMsCrmSettings msCrmSettings,
                              IUserContext userContext,
                              ICommonLog logger,
                              IUIServicesManager uiServicesManager,
                              IAPIOperationsServiceSettings operationsServiceSettings,
                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                              IGetBaseCurrencyService getBaseCurrencyService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                specialOperationsServiceSettings,
                getBaseCurrencyService)
        {
            _uiServicesManager = uiServicesManager;
        }

        public ActionResult View(IEntityType entityTypeName, IEntityType parentEntityType, string parentEntityId, string parentEntityState)
        {
            var entityId = !string.IsNullOrEmpty(parentEntityId) ? long.Parse(parentEntityId) : (long?)null;

            var uiService = _uiServicesManager.GetEntityGridViewService(entityTypeName);
            var gridViewSettings = uiService.GetGridViewSettings(UserContext.Profile);
            gridViewSettings = uiService.SecureViewsToolbars(gridViewSettings, entityId, parentEntityType, parentEntityState);

            return View("View", gridViewSettings);
        }

        public ActionResult Search(IEntityType entityTypeName)
        {
            var uiService = _uiServicesManager.GetEntityGridViewService(entityTypeName);
            var gridViewSettings = uiService.GetGridViewSettings(UserContext.Profile);
            
            return View("Search", gridViewSettings);
        }

        public ActionResult SearchMultiple(IEntityType entityTypeName)
        {
            var uiService = _uiServicesManager.GetEntityGridViewService(entityTypeName);
            var gridViewSettings = uiService.GetGridViewSettings(UserContext.Profile);

            return View("SearchMultiple", gridViewSettings);
        }
    }
}