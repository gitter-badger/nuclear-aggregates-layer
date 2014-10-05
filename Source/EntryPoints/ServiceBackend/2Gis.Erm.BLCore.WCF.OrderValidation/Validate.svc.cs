using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.WCF.OrderValidation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class OrderValidationApplicationService : IOrderValidationApplicationService, IOrderValidationApplicationRestService
    {
        private readonly IValidateOrdersOperationService _validateOrdersOperationService;
        private readonly IOrderValidationPredicateFactory _orderValidationPredicateFactory;

        public OrderValidationApplicationService(IUserContext userContext,
                                                 IValidateOrdersOperationService validateOrdersOperationService,
                                                 IOrderValidationPredicateFactory orderValidationPredicateFactory,
                                                 IResourceGroupManager resourceGroupManager)
        {
            _validateOrdersOperationService = validateOrdersOperationService;
            _orderValidationPredicateFactory = orderValidationPredicateFactory;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        public ValidationResult ValidateSingleOrder(string specifiedOrderId)
        {
            long orderId;
            if (!long.TryParse(specifiedOrderId, out orderId))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order Id cannot be parsed"), HttpStatusCode.BadRequest);
            }

            return _validateOrdersOperationService.Validate(orderId);
        }

        public ValidationResult ValidateSingleOrder(string specifiedOrderId, string specifiedNewOrderState)
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

            return _validateOrdersOperationService.Validate(orderId, (OrderState)newOrderState);
        }

        public ValidationResult ValidateSingleOrder(long orderId)
        {
            return _validateOrdersOperationService.Validate(orderId);
        }

        public ValidationResult ValidateOrders(ValidationType validationType, long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants)
        {
            return _validateOrdersOperationService.Validate(validationType, organizationUnitId, period, ownerCode, includeOwnerDescendants);
        }
    }
}
