using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateBillNumberService : IInvariantSafeCrosscuttingService
    {
        string Evaluate(string acquiredBillNumber, string orderNumber, int billIndex);
        string Evaluate(string acquiredBillNumber, string orderNumber);
    }
}