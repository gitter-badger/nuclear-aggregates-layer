using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations
{
    public interface IOperationsBoundElement : IMetadataElementAspect
    {
        bool HasOperations { get; }
        IEnumerable<OperationFeature> OperationFeatures { get; }
    }
}