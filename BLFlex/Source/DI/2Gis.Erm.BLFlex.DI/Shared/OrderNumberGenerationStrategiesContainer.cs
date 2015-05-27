using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders;

namespace DoubleGis.Erm.BLFlex.DI.Shared
{
    // Порядок стратегий имеет значение. 
    internal static class OrderNumberGenerationStrategiesContainer
    {
        public static IEnumerable<IOrderNumberGenerationStrategy> StrategiesForCyrillicAlphabetCountries = new IOrderNumberGenerationStrategy[]
                                                                                                               {
                                                                                                                   new ReadFromNewFormatOrderNumberGenerationStrategy(),
                                                                                                                   new ReadFromOldFormatOrderNumberGenerationStrategy(),
                                                                                                                   new UseReservedNumberOrderNumberGenerationStrategy(),
                                                                                                                   new UseExistingOrderNumberGenerationStrategy()
                                                                                                               };

        public static IEnumerable<IOrderNumberGenerationStrategy> StrategiesForRomanAlphabetCountries = new IOrderNumberGenerationStrategy[]
                                                                                                            {
                                                                                                                new ReadFromCurrentOrderNumberGenerationStrategy(),
                                                                                                                new UseReservedNumberOrderNumberGenerationStrategy(),
                                                                                                                new UseExistingOrderNumberGenerationStrategy()
                                                                                                            };

    }
}