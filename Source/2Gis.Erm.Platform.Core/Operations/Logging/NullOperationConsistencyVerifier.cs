using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class NullOperationConsistencyVerifier : IOperationConsistencyVerifier
    {
        public bool IsOperationContextConsistent(IEnumerable<IVerifierContext> verifierContexts)
        {
            return true;
        }
    }
}