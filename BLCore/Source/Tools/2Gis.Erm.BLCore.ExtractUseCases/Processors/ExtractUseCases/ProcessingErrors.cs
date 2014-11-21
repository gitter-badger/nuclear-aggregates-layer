using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases
{
    public class ProcessingErrors
    {
        public ProcessingErrors(IEnumerable<string> handlers, IEnumerable<string> requests)
        {
            NotCalledHandlers = new HashSet<string>(handlers);
            NotUsedRequests = new HashSet<string>(requests);
            NotConnectedUseCases = new List<Tuple<string, UseCaseEndpointDescriptor>>();
        }

        public ICollection<Tuple<string, UseCaseEndpointDescriptor>> NotConnectedUseCases { get; private set; }
        public HashSet<string> NotCalledHandlers { get; private set; }
        public HashSet<string> NotUsedRequests { get; private set; }
    }
}