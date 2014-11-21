using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class NullOperationConsistencyVerifier : IOperationConsistencyVerifier
    {
        public bool IsOperationContextConsistent(IEnumerable<IVerifierContext> verifierContexts)
        {
            return true;
        }
    }
}