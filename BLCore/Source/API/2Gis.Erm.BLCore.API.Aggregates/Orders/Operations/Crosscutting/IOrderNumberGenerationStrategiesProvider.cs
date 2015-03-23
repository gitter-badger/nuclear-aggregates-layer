using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IOrderNumberGenerationStrategiesProvider : IInvariantSafeCrosscuttingService
    {
        IEnumerable<IOrderNumberGenerationStrategy> GetStrategies();
    }
}