using System;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class OperationScopeNode
    {
        private readonly StrictOperationIdentity _operationIdentity;
        private readonly bool _isRoot;
        private readonly Guid _scopeId;
        private readonly EntityChangesContext _changesContext = new EntityChangesContext();
 
        public OperationScopeNode(Guid scopeId, bool isRoot, StrictOperationIdentity operationIdentity)
            : this(scopeId, isRoot, operationIdentity, new EntityChangesContext())
        {
        }

        public OperationScopeNode(Guid scopeId, bool isRoot, StrictOperationIdentity operationIdentity, EntityChangesContext changesContext)
        {
            _operationIdentity = operationIdentity;
            _isRoot = isRoot;
            _scopeId = scopeId;
            _changesContext = changesContext;
        }

        public EntityChangesContext ChangesContext
        {
            get { return _changesContext; }
        }

        public StrictOperationIdentity OperationIdentity
        {
            get { return _operationIdentity; }
        }

        public Guid ScopeId
        {
            get { return _scopeId; }
        }

        public bool IsRoot
        {
            get { return _isRoot; }
        }
    }
}