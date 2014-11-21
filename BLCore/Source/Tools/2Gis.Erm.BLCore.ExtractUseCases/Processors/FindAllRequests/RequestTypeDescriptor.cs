using Roslyn.Compilers.Common;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests
{
    public class RequestTypeDescriptor
    {
        public string RequestKey { get; set; }
        public INamedTypeSymbol RequestType { get; set; }
    }
}