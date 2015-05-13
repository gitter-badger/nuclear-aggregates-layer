using System;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using NuClear.Storage;
using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.OrderValidation
{
    [UseCase(Duration = UseCaseDuration.ExtraLong)]
    public sealed class AdvertisementsOnlyWhiteListOrderValidationRuleTest : IIntegrationTest
    {
        private readonly IFinder _finder;
        private readonly IOrderValidationPredicateFactory _orderValidationPredicateFactory;
        private readonly IUseCaseTuner _useCaseTuner;

        public AdvertisementsOnlyWhiteListOrderValidationRuleTest(
            IFinder finder, 
            IOrderValidationPredicateFactory orderValidationPredicateFactory,
            IUseCaseTuner useCaseTuner)
        {
            _finder = finder;
            _orderValidationPredicateFactory = orderValidationPredicateFactory;
            _useCaseTuner = useCaseTuner;
        }

        public ITestResult Execute()
        {
            _useCaseTuner.AlterDuration<AdvertisementsOnlyWhiteListOrderValidationRuleTest>();

            var targetMonthStart = new DateTime(2014, 11, 1);
            var targetTimePeriod = new TimePeriod(targetMonthStart, targetMonthStart.AddMonths(1).AddSeconds(-1));
            var validationParams = new MassOrdersValidationParams(Guid.NewGuid(), ValidationType.PreReleaseFinal)
                                       {
                                           Period = targetTimePeriod,
                                           OrganizationUnitId = 6,
                                           IncludeOwnerDescendants = false,
                                           OwnerId = null
                                       };

            var combinedPredicate = _orderValidationPredicateFactory.CreatePredicate(validationParams);

            IOrderValidationRule rule = new AdvertisementsOnlyWhiteListOrderValidationRule(_finder);
            var ruleMessages = rule.Validate(validationParams, combinedPredicate, null);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}
