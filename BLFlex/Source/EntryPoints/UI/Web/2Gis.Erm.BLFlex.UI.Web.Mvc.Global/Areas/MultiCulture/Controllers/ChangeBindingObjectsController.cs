using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Tracing.API;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.MultiCulture.Controllers
{
    public sealed class ChangeBindingObjectsController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IChangeOrderPositionBindingObjectsOperationService _changeOrderPositionBindingObjectsOperationService;

        public ChangeBindingObjectsController(IMsCrmSettings msCrmSettings,
                                              IAPIOperationsServiceSettings operationsServiceSettings,
                                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                              IIdentityServiceClientSettings identityServiceSettings,
                                              IUserContext userContext,
                                              ITracer tracer,
                                              IGetBaseCurrencyService getBaseCurrencyService,
                                              IOperationServicesManager operationServicesManager,
                                              IChangeOrderPositionBindingObjectsOperationService changeOrderPositionBindingObjectsOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, tracer, getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _changeOrderPositionBindingObjectsOperationService = changeOrderPositionBindingObjectsOperationService;
        }

        [HttpGet]
        public ActionResult ChangeBindingObjects(long positionId)
        {
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityName.OrderPosition);
            var domainEntityDto = service.GetDomainEntityDto(positionId, true, null, EntityName.None, null);

            // TODO {all, 05.05.2014}: Поменять модель и перенести в BL
            var model = new MultiCultureOrderPositionViewModel();
            model.LoadDomainEntityDto(domainEntityDto);
            model.IsLocked = false;

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeBindingObjects(long positionId, AdvertisementDescriptor[] advertisements)
        {
            _changeOrderPositionBindingObjectsOperationService.Change(positionId, advertisements);
            return new EmptyResult();
        }
    }
}
