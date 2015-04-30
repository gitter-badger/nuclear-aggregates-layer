using System;

namespace NuClear.Storage.Core
{
    public sealed class DomainContextHost : IDomainContextHost, IDomainContextsScopeFactory, IPendingChangesMonitorable
    {
        private readonly Guid _scopeId = Guid.NewGuid();
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        private bool _isDisposed;

        public DomainContextHost(ScopedDomainContextsStore scopedDomainContextsStore, IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
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

        IDomainContextsScope IDomainContextsScopeFactory.CreateScope()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("DomainContextHost was already disposed");
            }

            return new DomainContextsScope(_scopedDomainContextsStore, _pendingChangesHandlingStrategy);
        }

        void IDisposable.Dispose()
        {
            _pendingChangesHandlingStrategy.HandlePendingChanges(this);

            _scopedDomainContextsStore.Dispose();
            _isDisposed = true;
        }
    }
}