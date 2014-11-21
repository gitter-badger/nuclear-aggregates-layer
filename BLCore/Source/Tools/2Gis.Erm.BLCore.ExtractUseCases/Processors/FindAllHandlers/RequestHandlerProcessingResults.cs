using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers
{
    public class RequestHandlerProcessingResults
    {
        public IDictionary<string, HandlerDescriptor> ProcessedHandlers { get; set; }
        public IDictionary<string, HandlerDescriptor> InvalidProcessedHandlers { get; set; }

        public int FoundRequestHandlers { get; set; }
        public int InvalidRequestHandlers { get; set; }
    }
}