using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests;

using Roslyn.Compilers.Common;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls
{
    public class SubRequestDescriptor
    {
        public RequestTypeDescriptor TypeDescriptor { get; set; }
        public INamedTypeSymbol ContainingType { get; set; }
        public bool IsResumed { get; set; }
    }
}