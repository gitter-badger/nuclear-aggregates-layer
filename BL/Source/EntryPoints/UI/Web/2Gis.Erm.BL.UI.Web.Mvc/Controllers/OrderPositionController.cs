using System;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using Newtonsoft.Json;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    public class OrderPositionController : ControllerBase
    {
        private readonly IPublicService _publicService;
        private readonly IGetRatedPricesForCategoryOperationService _getRatedPricesForCategoryOperationService;
        private readonly IViewOrderPositionOperationService _viewOrderPositionOperationService;

        public OrderPositionController(IMsCrmSettings msCrmSettings,
                                       IAPIOperationsServiceSettings operationsServiceSettings,
                                       IAPISpecialOperationsServiceSettings specialOperationsServiceSettings,
                                       IAPIIdentityServiceSettings identityServiceSettings,
                                       IUserContext userContext,
                                       ICommonLog logger,
                                       IGetBaseCurrencyService getBaseCurrencyService,
                                       IPublicService publicService,
                                       IGetRatedPricesForCategoryOperationService getRatedPricesForCategoryOperationService,
                                       IViewOrderPositionOperationService viewOrderPositionOperationService)
            : base(msCrmSettings, operationsServiceSettings, specialOperationsServiceSettings, identityServiceSettings, userContext, logger, getBaseCurrencyService)
        {
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
        public JsonNetResult GetRatedPrices(long orderId, long pricePositionId, string categoryIds)
        {
            var decodedCategoryIds = JsonConvert.DeserializeObject<long[]>(categoryIds);
            var prices = _getRatedPricesForCategoryOperationService.GetRatedPrices(orderId, pricePositionId, decodedCategoryIds);
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
    }
}


