using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IOrderNumberGenerationStrategy : IInvariantSafeCrosscuttingService
    {
        bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber);
    }
}