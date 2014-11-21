using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls
{
    public class SubRequestsProcessingResult
    {
        public IDictionary<string, ICollection<SubRequestDescriptor>> SubRequests { get; set; }
        public IDictionary<string, ICollection<SubRequestDescriptor>> InvalidProcessedSubRequests { get; set; }

        public int FoundSubRequests { get; set; }
        public int InvalidSubRequests { get; set; }
    }
}