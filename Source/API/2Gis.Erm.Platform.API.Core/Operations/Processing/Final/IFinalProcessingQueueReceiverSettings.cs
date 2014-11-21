using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public interface IFinalProcessingQueueReceiverSettings : IMessageReceiverSettings
    {
        int ReprocessingBatchSize { get; }
    }
}