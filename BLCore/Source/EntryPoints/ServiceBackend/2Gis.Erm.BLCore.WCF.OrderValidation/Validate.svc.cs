using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.Platform.API.Core;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Common.Utils.Resources;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.WCF.OrderValidation
{
    // TODO {i.maslennikov, 20.10.2014}: Зачем в этом сервисе явное логирование ошибок? Есть же специальный обработчик исключений на уровне WCF, может быть его научить логировать семантически?
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
    public class OrderValidationApplicationService : IOrderValidationApplicationService, IOrderValidationApplicationRestService
    {
        private readonly IValidateOrdersOperationService _validateOrdersOperationService;
        private readonly ITracer _tracer;

        public OrderValidationApplicationService(IUserContext userContext,
                                                 IValidateOrdersOperationService validateOrdersOperationService,
                                                 IResourceGroupManager resourceGroupManager,
                                                 ITracer tracer)
        {
            _validateOrdersOperationService = validateOrdersOperationService;
            _tracer = tracer;

            resourceGroupManager.SetCulture(userContext.Profile.UserLocaleInfo.UserCultureInfo);
        }

        ValidationResult IOrderValidationApplicationRestService.ValidateSingleOrder(string specifiedOrderId)
        {
            long orderId;
            if (!long.TryParse(specifiedOrderId, out orderId))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order Id cannot be parsed"), HttpStatusCode.BadRequest);
            }

            try
            {
                return _validateOrdersOperationService.Validate(orderId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Validate single order {0} failed. ERM WCF Rest OrderValidation", orderId);
                throw;
            }
        }

        ValidationResult IOrderValidationApplicationRestService.ValidateSingleOrder(string specifiedOrderId, string specifiedNewOrderState)
        {
            long orderId;
            if (!long.TryParse(specifiedOrderId, out orderId))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order Id cannot be parsed"), HttpStatusCode.BadRequest);
            }

            int rawNewOrderState;
            if (!int.TryParse(specifiedNewOrderState, out rawNewOrderState))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException("Order State cannot be parsed"), HttpStatusCode.BadRequest);
            }

            if (!Enum.IsDefined(typeof(OrderState), rawNewOrderState))
            {
                throw new WebFaultException<ArgumentException>(new ArgumentException(string.Format("Unrecognized Order State: {0}", rawNewOrderState)), HttpStatusCode.BadRequest);
            }

            var newOrderState = (OrderState)rawNewOrderState;

            try
            {
                return _validateOrdersOperationService.Validate(orderId, newOrderState);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Validate single order {0} with new state {1} failed. ERM WCF Rest OrderValidation", orderId, newOrderState);
                throw;
            }
        }

        ValidationResult IOrderValidationApplicationService.ValidateSingleOrder(long orderId)
        {
            try
            {
                return _validateOrdersOperationService.Validate(orderId);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Validate single order {0} failed. ERM WCF Soap OrderValidation", orderId);
                throw;
            }
        }

        ValidationResult IOrderValidationApplicationService.ValidateOrders(ValidationType validationType, long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants)
        {
            try
            {
                return _validateOrdersOperationService.Validate(validationType, organizationUnitId, period, ownerCode, includeOwnerDescendants);
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(
                    ex, 
                    "Mass orders validation failed. Validation type: {0}. Organization unit: {1}. {2}. Owner code: {3}. Include owner descendants: {4}. ERM WCF Soap OrderValidation",
                    validationType,
                    organizationUnitId,
                    period,
                    ownerCode.HasValue ? ownerCode.Value.ToString() : "not specified",
                    includeOwnerDescendants);

                throw;
            }
        }
    }
}
