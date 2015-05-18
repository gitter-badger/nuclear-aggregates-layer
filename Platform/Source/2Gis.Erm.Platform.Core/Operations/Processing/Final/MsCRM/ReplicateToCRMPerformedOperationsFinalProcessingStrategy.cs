using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM
{
    public sealed class ReplicateToCRMPerformedOperationsFinalProcessingStrategy :
        MessageProcessingStrategyBase<FinalReplicate2MsCRMPerformedOperationsFlow, PerformedOperationsFinalProcessingMessage, ReplicateToCRMFinalProcessingResultsMessage>
    { 
        protected override ReplicateToCRMFinalProcessingResultsMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            return new ReplicateToCRMFinalProcessingResultsMessage
                {
                    EntityType = message.EntityName.AsEntityType(),
                    Ids = new[] { message.EntityId }
                };
        }
    }
}