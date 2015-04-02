using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders
{
    public sealed class RussiaOrderNumberGenerationStrategiesProvider : IOrderNumberGenerationStrategiesProvider
    {
        private readonly IEnumerable<IOrderNumberGenerationStrategy> _strategies = new IOrderNumberGenerationStrategy[]
                                                                                      {
                                                                                          new ReadFromNewFormatOrderNumberGenerationStrategy(),
                                                                                          new ReadFromOldFormatOrderNumberGenerationStrategy(),
                                                                                          new UseReservedNumberOrderNumberGenerationStrategy(),
                                                                                          new UseExistingOrderNumberGenerationStrategy()
                                                                                      };

        public IEnumerable<IOrderNumberGenerationStrategy> GetStrategies()
        {
            return _strategies;
        }
    }
}