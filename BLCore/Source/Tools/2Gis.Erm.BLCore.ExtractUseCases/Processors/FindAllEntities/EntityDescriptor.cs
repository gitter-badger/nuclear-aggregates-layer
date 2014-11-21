using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllEntities
{
    public class EntityDescriptor
    {
        public INamedTypeSymbol EntityType { get; set; }
        public DocumentId DeclaringDocument { get; set; }
        public bool IsDomainEntity { get; set; }
    }
}