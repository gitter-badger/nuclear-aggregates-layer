using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete
{
    public class EvaluateOrderNumberWithoutRegionalService : IEvaluateOrderNumberService
    {
        private readonly OrderNumberEvaluator _orderNumberEvaluator;

        public EvaluateOrderNumberWithoutRegionalService(string numberTemplate, IEnumerable<OrderNumberGenerationStrategy> generationStrategies)
        {
            _orderNumberEvaluator = new OrderNumberEvaluator(numberTemplate, generationStrategies);
        }

        public string Evaluate(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex)
        {
            return _orderNumberEvaluator.Evaluate(currentOrderNumber,
                                                  sourceOrganizationUnitSyncCode1C,
                                                  destinationOrganizationUnitSyncCode1C,
                                                  generatedOrderIndex);
        }

        public string EvaluateRegional(string currentOrderNumber,
                                       string sourceOrganizationUnitSyncCode1C,
                                       string destinationOrganizationUnitSyncCode1C,
                                       long? generatedOrderIndex)
        {
            throw new NotSupportedException(BLResources.RegionalOrdersAreNotSupported);
        }
    }
}
