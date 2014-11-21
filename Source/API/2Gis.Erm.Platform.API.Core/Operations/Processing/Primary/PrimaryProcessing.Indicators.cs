using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public static class PrimaryProcessing
    {
        public static bool IsPerformedOperationsSourceFlow(this IMessageFlow messageFlow)
        {
            return messageFlow is IPerformedOperationsPrimaryProcessingFlow;
        }
    }
}
