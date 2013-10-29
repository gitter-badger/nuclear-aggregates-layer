using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IOperationConsistencyVerifier
    {
        bool IsOperationContextConsistent(IEnumerable<IVerifierContext> verifierContexts);
    }
}
