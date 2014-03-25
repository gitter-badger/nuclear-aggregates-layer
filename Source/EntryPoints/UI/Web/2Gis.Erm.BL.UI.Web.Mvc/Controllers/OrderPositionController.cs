using System;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class OrderPositionController : ControllerBase
    {
        private readonly IOperationServicesManager _operationServicesManager;
        private readonly IPublicService _publicService;
        private readonly IGetRatedPricesForCategoryOperationService _getRatedPricesForCategoryOperationService;
        private readonly IViewOrderPositionOperationService _viewOrderPositionOperationService;

        public OrderPositionController(
            IMsCrmSettings msCrmSettings,
            IUserContext userContext,
            ICommonLog logger,
            IOperationServicesManager operationServicesManager,
            IPublicService publicService,
            IAPIOperationsServiceSettings operationsServiceSettings,
            IGetBaseCurrencyService getBaseCurrencyService,
            IGetRatedPricesForCategoryOperationService getRatedPricesForCategoryOperationService,
            IViewOrderPositionOperationService viewOrderPositionOperationService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings,
                getBaseCurrencyService)
        {
            _operationServicesManager = operationServicesManager;
            _publicService = publicService;
            _getRatedPricesForCategoryOperationService = getRatedPricesForCategoryOperationService;
            _viewOrderPositionOperationService = viewOrderPositionOperationService;
        }

        [HttpGet]
        public JsonNetResult GetEditValues(long? orderPositionId, long orderId, long pricePositionId, bool includeHidden)
        {
            var orderPositionWithSchemaDto = _viewOrderPositionOperationService.ViewOrderPosition(orderId, pricePositionId, orderPositionId, includeHidden);

            return new JsonNetResult(orderPositionWithSchemaDto);
        }

        [HttpGet]
        public JsonNetResult GetRatedPrices(long orderId, long pricePositionId, long? categoryId)
        {
            var prices = _getRatedPricesForCategoryOperationService.GetRatedPrices(orderId, pricePositionId, categoryId);
            return new JsonNetResult(prices);
        }

        [HttpPost]
        public JsonNetResult DiscountRecalc(RecalculateOrderPositionDiscountRequest request)
        {
            try
            {
                var response = (RecalculateOrderPositionDiscountResponse)_publicService.Handle(request);
                return new JsonNetResult(response);
            }
            catch (Exception ex)
            {
                var tmpModel = new ViewModel();
                ModelUtils.OnException(this, Logger, tmpModel, ex);
                return new JsonNetResult(tmpModel.Message);
            }
        }

        [HttpGet]
        public ActionResult ChangeBindingObjects(long positionId)
        {
            var service = _operationServicesManager.GetDomainEntityDtoService(EntityName.OrderPosition);
            var domainEntityDto = service.GetDomainEntityDto(positionId, true, null, EntityName.None, null);

            var model = new OrderPositionViewModel();
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


