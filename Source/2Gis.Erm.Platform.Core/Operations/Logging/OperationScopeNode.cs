using System.Collections.Concurrent;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopeNode
    {
        private readonly IOperationScope _operationScope;
        private readonly EntityChangesContext _scopeChanges = new EntityChangesContext();
        private readonly ConcurrentBag<OperationScopeNode> _childs = new ConcurrentBag<OperationScopeNode>();
 
        public OperationScopeNode(IOperationScope operationScope)
        {
            _operationScope = operationScope;
        }

        public IOperationScope Scope
        {
            get { return _operationScope; }
        }

        public EntityChangesContext ScopeChanges
        {
            get { return _scopeChanges; }
        }
        
        public IEnumerable<OperationScopeNode> Childs
        {
            get { return _childs; }
        }

        public void AddChild(OperationScopeNode childNode)
        {
            _childs.Add(childNode);
        }
    }
}