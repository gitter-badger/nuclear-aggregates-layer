﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

using ControllerBase = DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.Base.ControllerBase;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Controllers
{
    // FIXME {all, 08.11.2013}: данное УГ приехало из 1.0 - в данному случае необходимо реализовать данную операцию через WCF сервис Operations, js будет взаимодействовать непорседственно с ним - необходимость в данном контроллере отпадет
    public class OrderProcessingRequestController : ControllerBase
    {
        private readonly ICancelOrderProcessingRequestOperationService _cancelOrderProcessingRequestOperationService;
        private readonly IGetOrderRequestMessagesOperationService _getOrderRequestMessagesOperationService;

        public OrderProcessingRequestController(IMsCrmSettings msCrmSettings,
                                                IUserContext userContext,
                                                ICommonLog logger,
                                                IAPIOperationsServiceSettings operationsServiceSettings,
                                                IGetBaseCurrencyService getBaseCurrencyService,
                                                ICancelOrderProcessingRequestOperationService cancelOrderProcessingRequestOperationService,
                                                IGetOrderRequestMessagesOperationService getOrderRequestMessagesOperationService)
            : base(
                msCrmSettings,
                userContext,
                logger,
                operationsServiceSettings, 
                getBaseCurrencyService)
        {
            _cancelOrderProcessingRequestOperationService = cancelOrderProcessingRequestOperationService;
            _getOrderRequestMessagesOperationService = getOrderRequestMessagesOperationService;
        }

        #region Ajax methods

        [HttpPost]
        public JsonNetResult CancelOrderProcessingRequest(long orderProcessingRequestId)
        {
            _cancelOrderProcessingRequestOperationService.CancelRequest(orderProcessingRequestId);

            return new JsonNetResult();
        }

        [HttpGet]
        public JsonNetResult GetMessages(long orderProcessingRequestId)
        {
            var messages = _getOrderRequestMessagesOperationService.GetRequestMessages(orderProcessingRequestId);
            return new JsonNetResult(messages);
        }

        #endregion
    }
}
