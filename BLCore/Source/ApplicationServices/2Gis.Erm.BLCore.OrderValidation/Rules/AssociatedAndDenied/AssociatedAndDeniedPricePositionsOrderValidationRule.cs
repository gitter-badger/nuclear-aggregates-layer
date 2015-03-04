using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    /// <summary>
    /// Проверить являются ли позиции заказа сопутствующими к позициям которых нет в выборке по активным заказам 
    /// И
    /// Проверить являются ли позиции заказа запрещёнными к какой либо из позиций присутствующей в выборке по активным заказам
    /// </summary>
    // TODO {all, 03.10.2014}: подумать, над упрощением реализации данной проверки, например, как жить дальше будет CheckOrdersAssociatedAndDeniedPositionsRequest и будет ли (ранне это был отдельный handler, теперь все это переехало в данный rule )
    public sealed class AssociatedAndDeniedPricePositionsOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;
        private readonly IPriceConfigurationService _priceConfigurationService;
        private readonly ITracer _tracer;

        public AssociatedAndDeniedPricePositionsOrderValidationRule(
            IFinder finder,
            IPriceConfigurationService priceConfigurationService,
            ITracer tracer)
        {
            _finder = finder;
            _priceConfigurationService = priceConfigurationService;
            _tracer = tracer;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            CheckOrdersAssociatedAndDeniedPositionsRequest checkRequest;
            switch (ruleContext.ValidationParams.Type)
            {
                // Для единичной проверки определен id заказа
                case ValidationType.SingleOrderOnRegistration:
                    checkRequest = CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequest(ruleContext.ValidationParams.Single.OrderId);
                    break;

                // Для единичной проверки определен id заказа
                case ValidationType.SingleOrderOnStateChanging:
                    checkRequest = CreateSingleCheckRequest(ruleContext.ValidationParams.Single.OrderId, ruleContext.ValidationParams.Single.CurrentOrderState, ruleContext.ValidationParams.Single.NewOrderState);
                    break;

                // Для массовой проверки определено отделение организации
                default:
                    checkRequest = CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForMassiveCheck(ruleContext.OrdersFilterPredicate);
                    break;
            }

            if (checkRequest == null)
            {
                return Enumerable.Empty<OrderValidationMessage>();
            }

            return ValidateAssociatedAndDenied(checkRequest);
        }

        private static CheckOrdersAssociatedAndDeniedPositionsRequest CreateSingleCheckRequest(long orderId, OrderState previousState, OrderState newState)
        {
            if (newState == OrderState.OnRegistration && (previousState == OrderState.OnApproval || previousState == OrderState.Approved))
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.OnTermination && previousState == OrderState.Approved)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.Rejected && previousState == OrderState.OnApproval)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderBeingCancelled(orderId);
            }

            if (newState == OrderState.Approved && previousState == OrderState.OnTermination)
            {
                return CheckOrdersAssociatedAndDeniedPositionsRequest.CreateRequestForOrderbeingReapproved(orderId);
            }

            return null;
        }

        private IEnumerable<OrderValidationMessage> ValidateAssociatedAndDenied(CheckOrdersAssociatedAndDeniedPositionsRequest request)
        {
            var messages = new List<OrderValidationMessage>();

            var validationQueryProvider = new ADPValidationQueryProvider(_finder, request.Mode, request.OrderId, request.FilterExpression);

            var orderStatesDictionary = ADPValidationInitializationHelper.LoadOrderStates(validationQueryProvider);

            var validationResultBuilder = new ADPValidationResultBuilder(orderStatesDictionary, request.Mode, request.OrderId);
            var validatorsDictionary = ADPValidationInitializationHelper.LoadValidators(_tracer,
                                                                                        request.Mode,
                                                                                        request.OrderId,
                                                                                        validationQueryProvider,
                                                                                        orderStatesDictionary,
                                                                                        _priceConfigurationService,
                                                                                        validationResultBuilder,
                                                                                        messages);
            switch (request.Mode)
            {
                case ADPCheckMode.SpecificOrder:
                    if (validatorsDictionary.Count > 0)
                    {
                        var validator = validatorsDictionary.Values.First();
                        validator.CheckSpecificOrder(request.OrderId);
                        messages.AddRange(validationResultBuilder.GetMessages());
                    }

                    break;
                case ADPCheckMode.OrderBeingCancelled:
                    if (validatorsDictionary.Count > 0)
                    {
                        var validator = validatorsDictionary.Values.First();
                        validator.CheckOrderBeingCancelled();
                        messages.AddRange(validationResultBuilder.GetMessages());
                    }

                    break;
                case ADPCheckMode.Massive:
                    foreach (var validator in validatorsDictionary.Values)
                    {
                        validator.MassiveCheckOrder();
                    }

                    messages.AddRange(validationResultBuilder.GetMessages());
                    break;
                case ADPCheckMode.OrderBeingReapproved:
                    if (validatorsDictionary.Count > 0)
                    {
                        var validator = validatorsDictionary.Values.First();
                        validator.CheckOrderBeingReapproved(request.OrderId);
                        messages.AddRange(validationResultBuilder.GetMessages());
                    }
                    break;
            }

            return messages;
        }

        private sealed class CheckOrdersAssociatedAndDeniedPositionsRequest
        {
            private CheckOrdersAssociatedAndDeniedPositionsRequest()
            {
            }

            public ADPCheckMode Mode { get; private set; }
            public long OrderId { get; private set; }
            public Expression<Func<Order, bool>> FilterExpression { get; private set; }

            public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequest(long orderId)
            {
                return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.SpecificOrder, OrderId = orderId };
            }

            public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForOrderBeingCancelled(long orderId)
            {
                return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.OrderBeingCancelled, OrderId = orderId };
            }

            public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForMassiveCheck(Expression<Func<Order, bool>> filterExpression)
            {
                return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.Massive, FilterExpression = filterExpression };
            }

            public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForOrderbeingReapproved(long orderId)
            {
                return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.OrderBeingReapproved, OrderId = orderId };
            }
        }
    }
}
