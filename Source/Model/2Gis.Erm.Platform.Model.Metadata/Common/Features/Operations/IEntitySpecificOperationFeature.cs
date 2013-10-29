using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public interface IEntitySpecificOperationFeature : IBoundOperationFeature
    {
        EntitySet Entity { get; set; }
    }
}