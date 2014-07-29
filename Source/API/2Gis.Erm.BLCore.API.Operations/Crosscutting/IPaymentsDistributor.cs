using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Operations.Crosscutting
{
    public interface IPaymentsDistributor : IInvariantSafeCrosscuttingService
    {
        decimal[] DistributePayment(int paymentsNumber, decimal totalAmount);
    }
}
