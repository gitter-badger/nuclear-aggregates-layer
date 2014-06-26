using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class OperationScopeNode
    {
        private readonly IOperationScope _operationScope;
        private readonly EntityChangesContext _scopeChanges = new EntityChangesContext();

        private readonly object _childsSync = new object();
        private readonly IList<OperationScopeNode> _childs = new List<OperationScopeNode>();
 
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
            get 
            {
                lock (_childsSync)
                {
                    return _childs.ToArray();
                }
            }
        }

        public void AddChild(OperationScopeNode childNode)
        {
            lock (_childsSync)
            {
                _childs.Add(childNode);
            }
        }
    }
}