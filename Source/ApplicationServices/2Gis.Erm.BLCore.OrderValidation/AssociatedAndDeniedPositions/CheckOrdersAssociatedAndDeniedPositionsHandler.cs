using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLCore.OrderValidation.AssociatedAndDeniedPositions
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public class CheckOrdersAssociatedAndDeniedPositionsHandler : RequestHandler<CheckOrdersAssociatedAndDeniedPositionsRequest, ValidateOrdersResponse>
    {
        private readonly ICommonLog _logger;
        private readonly IFinder _finder;
        private readonly IPriceConfigurationService _priceConfigurationService;

        public CheckOrdersAssociatedAndDeniedPositionsHandler(
            ICommonLog logger,
            IFinder finder,
            IPriceConfigurationService priceConfigurationService)
        {
            _logger = logger;
            _finder = finder;
            _priceConfigurationService = priceConfigurationService;
        }

        protected override ValidateOrdersResponse Handle(CheckOrdersAssociatedAndDeniedPositionsRequest request)
        {
            var messages = new List<OrderValidationMessage>();

            var validationQueryProvider = new ADPValidationQueryProvider(_finder, request.Mode, request.OrderId, request.FilterExpression);

            var orderStatesDictionary = ADPValidationInitializationHelper.LoadOrderStates(validationQueryProvider);

            var validationResultBuilder = new ADPValidationResultBuilder(orderStatesDictionary, request.Mode, request.OrderId);
            var validatorsDictionary = ADPValidationInitializationHelper.LoadValidators(_logger,
                                                                                        request.Mode,
                                                                                        request.OrderId,
                                                                                        validationQueryProvider,
                                                                                        orderStatesDictionary,
                                                                                        _priceConfigurationService,
                                                                                        validationResultBuilder,
                                                                                        messages);
            var stopwatch = Stopwatch.StartNew();

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

            stopwatch.Stop();
            _logger.DebugFormatEx("Проверка СЗП. Выполнение проверки: {0:F3}", stopwatch.ElapsedMilliseconds / 1000D);

            return new ValidateOrdersResponse(messages);
        }
    }
}
