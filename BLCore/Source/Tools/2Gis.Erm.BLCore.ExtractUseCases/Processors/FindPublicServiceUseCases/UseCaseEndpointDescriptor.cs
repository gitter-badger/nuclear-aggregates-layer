using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases
{
    public class UseCaseEndpointDescriptor
    {
        public string Description { get; set; }
        public INamedTypeSymbol RequestType { get; set; }
        public INamedTypeSymbol ContainingClass { get; set; }
        public ISymbol ContainingClassMember { get; set; }
        public SyntaxNode CallSyntaxTree { get; set; }
        public DocumentId DeclaringDocument { get; set; }
    }
}