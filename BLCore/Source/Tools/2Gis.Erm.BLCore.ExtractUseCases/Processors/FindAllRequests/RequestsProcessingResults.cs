using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests
{
    public class RequestsProcessingResults
    {
        public IDictionary<string, List<RequestDescriptor>> ProcessedRequests { get; set; }
        public IDictionary<string, List<RequestDescriptor>> InvalidProcessedRequests { get; set; }

        public int FoundRequests { get; set; }
        public int InvalidRequests { get; set; }
    }
}