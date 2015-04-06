using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Shared
{
    internal class OrderNumberEvaluator
    {
        private readonly IEnumerable<IOrderNumberGenerationStrategy> _generationStrategies;

        internal OrderNumberEvaluator(IEnumerable<IOrderNumberGenerationStrategy> generationStrategies)
        {
            _generationStrategies = generationStrategies;
        }

        public string Evaluate(string numberTemplate,
                               string currentOrderNumber,
                               string sourceOrganizationUnitSyncCode1C,
                               string destinationOrganizationUnitSyncCode1C,
                               long? generatedOrderIndex)
        {
            var numberFormatWithSyncCodes = string.Format(numberTemplate, sourceOrganizationUnitSyncCode1C, destinationOrganizationUnitSyncCode1C, "{0}");

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