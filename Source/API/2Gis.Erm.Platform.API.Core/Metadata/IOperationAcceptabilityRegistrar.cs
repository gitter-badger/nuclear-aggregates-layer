using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IOperationAcceptabilityRegistrar
    {
        IReadOnlyDictionary<int, OperationApplicability> InitialOperationApplicability { get; }
    }
}
