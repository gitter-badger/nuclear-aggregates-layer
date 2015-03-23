using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public class EvaluateOrderNumberWithoutRegionalService : IEvaluateOrderNumberService
    {
        private readonly OrderNumberEvaluator _orderNumberEvaluator;

        public EvaluateOrderNumberWithoutRegionalService(string numberTemplate, IEnumerable<IOrderNumberGenerationStrategy> generationStrategies)
        {
            _orderNumberEvaluator = new OrderNumberEvaluator(numberTemplate, generationStrategies);
        }

        public string Evaluate(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex, OrderType orderType)
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
            throw new NotSupportedException("");//BLResources.RegionalOrdersAreNotSupported);
        }
    }
}
