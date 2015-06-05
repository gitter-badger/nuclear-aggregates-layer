using System;

namespace NuClear.Storage.Core
{
    public sealed class DomainContextsScope : IDomainContextsScope, IPendingChangesMonitorable
    {
        private readonly Guid _scopeId = Guid.NewGuid();
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        private bool _isDisposed;
        
        public DomainContextsScope(ScopedDomainContextsStore scopedDomainContextsStore, IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            _scopedDomainContextsStore = scopedDomainContextsStore;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
        }

        Guid IDomainContextHost.ScopeId
        {
            get { return _scopeId; }
        }

        bool IPendingChangesMonitorable.AnyPendingChanges
        {
            get { return _scopedDomainContextsStore.AnyPendingChanges(this); }
        }

        void IDisposable.Dispose()
        {
            
            _pendingChangesHandlingStrategy.HandlePendingChanges(this);

            var modifiableDomainContexts = _scopedDomainContextsStore.DropModifiable(this);
            if (modifiableDomainContexts != null)
            {
                foreach (var domainContext in modifiableDomainContexts)
                {
                    domainContext.Dispose();
                }
            }

            _isDisposed = true;
        }

        void IDomainContextsScope.Complete()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("DomainContextsScope was already disposed");
            }

            if (!((IPendingChangesMonitorable)this).AnyPendingChanges)
            {
                return;
            }

            throw new InvalidOperationException("Some of the domain contexts hosted by scope has unsaved changes - check aggregates layer implementations");
        }
    }
}