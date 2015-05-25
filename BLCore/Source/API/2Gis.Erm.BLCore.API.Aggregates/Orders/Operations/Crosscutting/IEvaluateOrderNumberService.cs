using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IEvaluateOrderNumberService : IInvariantSafeCrosscuttingService
    {
        string Evaluate(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex, OrderType orderType);
        string EvaluateRegional(string currentOrderNumber, string sourceOrganizationUnitSyncCode1C, string destinationOrganizationUnitSyncCode1C, long? generatedOrderIndex);
    }
}