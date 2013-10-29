using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Operations
{
    public interface IOperationsBoundElement : IConfigElementAspect
    {
        bool HasOperations { get; }
        IEnumerable<IBoundOperationFeature> OperationFeatures { get; }
    }
}