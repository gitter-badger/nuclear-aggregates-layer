using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    internal class OrderNumberEvaluator
    {
        private readonly string _numberTemplate;
        private readonly IEnumerable<IOrderNumberGenerationStrategy> _generationStrategies;

        internal OrderNumberEvaluator(string numberTemplate, IEnumerable<IOrderNumberGenerationStrategy> generationStrategies)
        {
            _numberTemplate = numberTemplate;
            _generationStrategies = generationStrategies;
        }

        public string Evaluate(string currentOrderNumber,
                       string sourceOrganizationUnitSyncCode1C,
                       string destinationOrganizationUnitSyncCode1C,
                       long? generatedOrderIndex)
        {
            var numberFormatWithSyncCodes = string.Format(_numberTemplate, sourceOrganizationUnitSyncCode1C, destinationOrganizationUnitSyncCode1C, "{0}");

            foreach (var strategy in _generationStrategies)
            {
                string orderNumber;
                if (strategy.TryGenerateNumber(currentOrderNumber, numberFormatWithSyncCodes, generatedOrderIndex, out orderNumber))
                {
                    return orderNumber;
                }
            }

            throw new NotificationException(BLResources.FailedToGenerateOrderNumber);
        }
    }
}