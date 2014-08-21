namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public sealed class FinalProcessingQueueReceiverSettings : IFinalProcessingQueueReceiverSettings
    {
        public int BatchSize { get; set; }
        public bool IsRecoveryMode { get; set; }
    }
}