using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Shared
{
    internal static class OrderNumberStrategyExtensions
    {
        public static string Execute(this IEnumerable<IOrderNumberGenerationStrategy> generationStrategies,
                                     string numberTemplate,
                                     string currentOrderNumber,
                                     string sourceOrganizationUnitSyncCode1C,
                                     string destinationOrganizationUnitSyncCode1C,
                                     long? generatedOrderIndex)
        {
            var numberFormatWithSyncCodes = string.Format(numberTemplate, sourceOrganizationUnitSyncCode1C, destinationOrganizationUnitSyncCode1C, "{0}");

            foreach (var strategy in generationStrategies)
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