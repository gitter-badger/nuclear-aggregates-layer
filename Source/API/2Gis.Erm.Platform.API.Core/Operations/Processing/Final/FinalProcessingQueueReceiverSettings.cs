namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public sealed class FinalProcessingQueueReceiverSettings : IFinalProcessingQueueReceiverSettings
    {
        public int BatchSize { get; set; }
        public int ReprocessingBatchSize { get; set; }
    }
}