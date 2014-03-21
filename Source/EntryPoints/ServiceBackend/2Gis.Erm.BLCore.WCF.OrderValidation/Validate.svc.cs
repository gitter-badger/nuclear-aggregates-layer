using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.WCF.OrderValidation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class OrderValidationApplicationService : IOrderValidationApplicationService, IOrderValidationApplicationRestService
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IOrderValidationPredicateFactory _orderValidationPredicateFactory;

        public OrderValidationApplicationService(IBusinessModelSettings businessModelSettings,
                                                 IUserContext userContext,
                                                 IOrderValidationService orderValidationService,
                                                 IOrderValidationPredicateFactory orderValidationPredicateFactory)
        {
            _businessModelSettings = businessModelSettings;
            _orderValidationService = orderValidationService;
            _orderValidationPredicateFactory = orderValidationPredicateFactory;

            BLResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            MetadataResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
            EnumResources.Culture = userContext.Profile.UserLocaleInfo.UserCultureInfo;
        }

        public ValidateOrdersResult ValidateSingleOrder(string specifiedOrderId)
        {
            long orderId;
            if (!long.TryParse(specifiedOrderId, out orderId))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order Id cannot be parsed"), HttpStatusCode.BadRequest);
            }

            return ValidateSingleOrder(orderId);
        }

        public ValidateOrdersResult ValidateSingleOrder(string specifiedOrderId, string specifiedNewOrderState)
        {
            long orderId;
            if (!long.TryParse(specifiedOrderId, out orderId))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order Id cannot be parsed"), HttpStatusCode.BadRequest);
            }

            int newOrderState;
            if (!int.TryParse(specifiedNewOrderState, out newOrderState))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order State cannot be parsed"), HttpStatusCode.BadRequest);
            }

            if (!Enum.IsDefined(typeof(OrderState), newOrderState))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException(string.Format("Unrecognized Order State: {0}", newOrderState)), HttpStatusCode.BadRequest);
            }

            return ValidateSingleOrder(orderId, (OrderState)newOrderState);
        }

        public ValidateOrdersResult ValidateSingleOrder(long orderId)
        {
            return ValidateSingleOrder(orderId, null);
        }

        public ValidateOrdersResult ValidateOrders(ValidationType validationType, long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants)
        {
            var orderValidationPredicate = _orderValidationPredicateFactory.CreatePredicate(organizationUnitId, period, ownerCode, includeOwnerDescendants);
            var validateOrdersRequest = new ValidateOrdersRequest
                {
                    Type = validationType,
                    OrganizationUnitId = organizationUnitId,
                    Period = period,
                    OwnerId = ownerCode,
                    IncludeOwnerDescendants = includeOwnerDescendants,
                    SignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber
                };
            return _orderValidationService.ValidateOrders(orderValidationPredicate, validateOrdersRequest);
        }

        private ValidateOrdersResult ValidateSingleOrder(long orderId, OrderState? newOrderState)
        {
            OrderState currentOrderState;
            TimePeriod period;
            var orderValidationPredicate = _orderValidationPredicateFactory.CreatePredicate(orderId, out currentOrderState, out period);
            var validateOrdersRequest = new ValidateOrdersRequest
                {
                    Type = newOrderState == null || currentOrderState == OrderState.OnRegistration
                               ? ValidationType.SingleOrderOnRegistration
                               : ValidationType.SingleOrderOnStateChanging,
                    OrderId = orderId,
                    CurrentOrderState = currentOrderState,
                    NewOrderState = newOrderState == null ? OrderState.NotSet : newOrderState.Value,
                    Period = period,
                    SignificantDigitsNumber = _businessModelSettings.SignificantDigitsNumber
                };

            return _orderValidationService.ValidateOrders(orderValidationPredicate, validateOrdersRequest);
        }
    }
}
