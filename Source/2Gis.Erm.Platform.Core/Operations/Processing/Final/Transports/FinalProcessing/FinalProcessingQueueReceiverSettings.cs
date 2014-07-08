namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Final.Transports.FinalProcessing
{
    public sealed class FinalProcessingQueueReceiverSettings : IFinalProcessingQueueReceiverSettings
    {
        public int BatchSize { get; set; }
        public bool IsRecoveryMode { get; set; }
    }
}