namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.ProcessingResults
{
    public interface IProcessingResultsMessage
    {
        string Text { get; set; }
        ProcessingResultsMessageType Type { get; set; }
    }
}