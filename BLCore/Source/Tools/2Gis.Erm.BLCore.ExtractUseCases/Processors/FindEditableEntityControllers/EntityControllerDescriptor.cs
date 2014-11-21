using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindEditableEntityControllers
{
    public class EntityControllerDescriptor
    {
        public INamedTypeSymbol ControllerType { get; set; }
        public INamedTypeSymbol EntityType { get; set; }
        public string EntityKey { get; set; }
        public DocumentId DeclaringDocument { get; set; }
    }
}