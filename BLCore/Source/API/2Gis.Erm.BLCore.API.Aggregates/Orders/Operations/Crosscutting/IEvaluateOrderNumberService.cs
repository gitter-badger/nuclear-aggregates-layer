using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateOrderNumberService : IInvariantSafeCrosscuttingService
    {
        string Evaluate(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex);
        string EvaluateRegional(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex);
    }
}