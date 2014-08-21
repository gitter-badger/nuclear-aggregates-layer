using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports.FinalProcessing
{
    public interface IFinalProcessingQueueReceiverSettings : IMessageReceiverSettings
    {
        int ReprocessingBatchSize { get; }
    }
}