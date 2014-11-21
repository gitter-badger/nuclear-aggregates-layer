using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases
{
    public class UseCaseProcessingResults
    {
        public IDictionary<string, ICollection<UseCaseEndpointDescriptor>> ProcessedUseCases { get; set; }
        public IDictionary<string, ICollection<UseCaseEndpointDescriptor>> InvalidProcessedUseCases { get; set; }

        public int FoundUseCases { get; set; }
        public int InvalidUseCases { get; set; }
    }
}