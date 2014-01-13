using System.Collections.Generic;

using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests
{
    public class RequestDescriptor
    {
        public RequestDescriptor()
        {
            TypeVariations = new List<RequestTypeDescriptor>();
        }

        public RequestTypeDescriptor TypeDescriptor { get; set; }
        public INamedTypeSymbol[] InheritanceChain { get; set; }
        public List<RequestTypeDescriptor> TypeVariations { get; private set; }
        public DocumentId DeclaringDocument { get; set; }
    }
}