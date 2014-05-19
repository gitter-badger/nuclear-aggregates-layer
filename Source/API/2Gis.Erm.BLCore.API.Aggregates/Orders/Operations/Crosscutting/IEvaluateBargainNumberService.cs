using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateBargainNumberService : IInvariantSafeCrosscuttingService
    {
        string Evaluate(string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long bargainUniqueIndex);
    }
}