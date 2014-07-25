using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Aggregates.ReadModel.Orders.Validation
{
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class OnlyWhiteListReadModelTest : IIntegrationTest
    {
        private const bool IsCheckMassive = true;

        private readonly IFinder _finder;
        private readonly IUseCaseTuner _useCaseTuner;

        public OnlyWhiteListReadModelTest(
            IFinder finder,
            IUseCaseTuner useCaseTuner)
        {
            _finder = finder;
            _useCaseTuner = useCaseTuner;
        }

        private FindSpecification<Order> AlreadyValidatedFilter
        {
            get
            {
                return new FindSpecification<Order>(o => !o.OrderValidationResults
                                                                           .Where(y => y.OrderValidationGroupId == 2)
                                                                           .OrderByDescending(y => y.Id)
                                                                           .Select(y => y.IsValid)
                                                                           .FirstOrDefault());
            }
        }

        public ITestResult Execute()
        {
            _useCaseTuner.AlterDuration<OnlyWhiteListReadModelTest>();

            const int DestinationOrganizationUnit = 10;
            var periodStart = new DateTime(2014, 8, 1);
            var releasePeriod = new TimePeriod(periodStart, periodStart.AddMonths(1).Subtract(TimeSpan.FromSeconds(1)));

            var validateOrdersRequest = new ValidateOrdersRequest
            {
                Type = ValidationType.PreReleaseBeta,
                OrganizationUnitId = DestinationOrganizationUnit,
                Period = releasePeriod,
                OwnerId = null,
                IncludeOwnerDescendants = false,
                SignificantDigitsNumber = 2
            };

            var messages = new List<OrderValidationMessage>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ExecuteOriginalRule(
                validateOrdersRequest,
                //GetOrdersAsInReleasingAsValidationPredicate(DestinationOrganizationUnit, releasePeriod),
                GetOrdersAsInOrderValidationAsValidationPredicate(DestinationOrganizationUnit, releasePeriod),
                Enumerable.Empty<long>(),
                messages);
            
            stopwatch.Stop();

            var messagesCount = messages.Count;
            var originalRuleExecutionTime = stopwatch.Elapsed.TotalSeconds;

            
            return OrdinaryTestResult.As.Succeeded;
        }

        private OrderValidationPredicate GetOrdersAsInReleasingAsValidationPredicate(int destinationOrganizationUnitId, TimePeriod timePeriod)
        {
            //_finder.Find(OrderSpecs.Orders.Find.ForRelease(destinationOrganizationUnitId, timePeriod) && AlreadyValidatedFilter);

            var resultPredicate =
                new OrderValidationPredicate(
                    (Specs.Find.ActiveAndNotDeleted<Order>() 
                    && OrderSpecs.Orders.Find.ByPeriod(timePeriod) 
                    && OrderSpecs.Orders.Find.ReleasableStatuses).Predicate,
                    o => o.DestOrganizationUnitId == destinationOrganizationUnitId,
                    AlreadyValidatedFilter.Predicate);

            return resultPredicate;
        }

        private OrderValidationPredicate GetOrdersAsInOrderValidationAsValidationPredicate(int destinationOrganizationUnitId, TimePeriod timePeriod)
        {
            var orderValidationPredicateFactory = new OrderValidationPredicateFactory(_finder);
            var orderFilterPredicate = orderValidationPredicateFactory.CreatePredicate(destinationOrganizationUnitId, timePeriod, null, false);

            var combinedPredicate = new OrderValidationPredicate(orderFilterPredicate.GeneralPart,
                                                                 orderFilterPredicate.OrgUnitPart,
                                                                 AlreadyValidatedFilter.Predicate);

            return combinedPredicate;
        }

        private void ExecuteOriginalRule(
            ValidateOrdersRequest validateOrdersRequest, 
            OrderValidationPredicate validationPredicate, 
            IEnumerable<long> invalidOrders, 
            List<OrderValidationMessage> messages)
        {
            IOrderValidationRule targetValidationRule = null;//new AdvertisementsOnlyWhiteListOrderValidationRule(_finder);
            var results = targetValidationRule.Validate(validationPredicate, invalidOrders, validateOrdersRequest);
            messages.AddRange(results);
        }
    }
}
