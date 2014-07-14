using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IUseCaseTracker
    {
        void AddOperation(OperationScopeNode operationScopeNode);
        void AttachToParent(Guid parentOperationScopeId, params Guid[] childOperationScopeId);
        void AttachToParent(Guid parentOperationScopeId, IEnumerable<Guid> childOperationScopeId);
        void Complete();
    }
}
