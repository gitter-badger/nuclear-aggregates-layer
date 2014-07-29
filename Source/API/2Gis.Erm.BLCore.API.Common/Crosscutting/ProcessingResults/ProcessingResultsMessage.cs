using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.ProcessingResults
{
    [DataContract]
    public sealed class ProcessingResultsMessage : IProcessingResultsMessage
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public ProcessingResultsMessageType Type { get; set; }
    }
}
