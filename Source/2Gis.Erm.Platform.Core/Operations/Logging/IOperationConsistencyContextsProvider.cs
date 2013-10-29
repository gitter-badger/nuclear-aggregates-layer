using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public interface IOperationConsistencyContextsProvider
    {
        IEnumerable<IVerifierContext> GetContexts(OperationScopeNode operationContextsRoot);
    }
}