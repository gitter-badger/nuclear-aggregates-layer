using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.MultiCulture.Controllers
{
    public sealed class ChangeBindingObjectsController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IPublicService _publicService;

        public ChangeBindingObjectsController(IMsCrmSettings msCrmSettings,
                                              IUserContext userContext,
                                              ICommonLog logger,
                                              IAPIOperationsServiceSettings operationsServiceSettings,
                                              IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                              IGetBaseCurrencyService getBaseCurrencyService,
                                              IOperationServicesManager operationServicesManager,
                                              IPublicService publicService)
            : base(msCrmSettings,
                   userContext,
                   logger,
                   operationsServiceSettings,
                   specialOperationsServiceSettings,
                   getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _publicService = publicService;
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
            var request = new ChangeBindingObjectsRequest
            {
                OrderPositionId = positionId,
                Advertisements = advertisements
            };

            _publicService.Handle(request);
            return new EmptyResult();
        }
    }
}
