using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.Storage.Core
{
    public sealed class ScopedDomainContextsStore : IDisposable
    {
        private readonly object _sync = new object();

        private readonly IReadDomainContext _readDomainContext;
        private readonly IModifiableDomainContextFactory _modifiableDomainContextFactory;
        private readonly IDictionary<Guid, HostDomainContextsStorage> _domainContextRegistrar = new Dictionary<Guid, HostDomainContextsStorage>();

        private bool _isDisposed;

        public ScopedDomainContextsStore(IReadDomainContext readDomainContext, IModifiableDomainContextFactory modifiableDomainContextFactory)
        {
            _readDomainContext = readDomainContext;
            _modifiableDomainContextFactory = modifiableDomainContextFactory;
        }

        public IReadDomainContext GetReadable(IDomainContextHost domainContextHost)
        {
            lock (_sync)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (!_domainContextRegistrar.TryGetValue(domainContextHost.ScopeId, out hostDomainContextsStorage))
                {
                    hostDomainContextsStorage = new HostDomainContextsStorage(_readDomainContext);
                    _domainContextRegistrar.Add(domainContextHost.ScopeId, hostDomainContextsStorage);
                }
            }

            return _readDomainContext;
        }

        public IModifiableDomainContext GetModifiable<TEntity>(IDomainContextHost domainContextHost) where TEntity : class
        {
            // важно т.к. domaincontext привязываются к (соответствуют) сущностным репозиториям
            var targetEntityType = typeof(TEntity); 
            IModifiableDomainContext domainContext;

            lock (_sync)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (!_domainContextRegistrar.TryGetValue(domainContextHost.ScopeId, out hostDomainContextsStorage))
                {
                    hostDomainContextsStorage = new HostDomainContextsStorage(_readDomainContext);
                    _domainContextRegistrar.Add(domainContextHost.ScopeId, hostDomainContextsStorage);
                }

                var modifiableDomainContexts = hostDomainContextsStorage.ModifiableDomainContexts;
                if (!modifiableDomainContexts.TryGetValue(targetEntityType, out domainContext))
                {   
                    // контекста нет, нужно создать
                    domainContext = _modifiableDomainContextFactory.Create<TEntity>();
                    modifiableDomainContexts.Add(targetEntityType, domainContext);
                }
            }

            return domainContext;
        }

        public IEnumerable<IModifiableDomainContext> DropModifiable(IDomainContextHost host)
        {
            lock (_sync)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (_domainContextRegistrar.TryGetValue(host.ScopeId, out hostDomainContextsStorage))
                {
                    _domainContextRegistrar.Remove(host.ScopeId);
                    return hostDomainContextsStorage.ModifiableDomainContexts.Values.ToArray();
                }

                return Enumerable.Empty<IModifiableDomainContext>();
            }
        }

        public bool AnyPendingChanges(IDomainContextHost domainContextHost)
        {
            lock (_sync)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (_domainContextRegistrar.TryGetValue(domainContextHost.ScopeId, out hostDomainContextsStorage))
                {
                    return hostDomainContextsStorage.ModifiableDomainContexts.Values.Any(c => c.AnyPendingChanges);
                }

                return false;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            lock (_sync)
            {
                if (_domainContextRegistrar.Count > 0)
                {
                    foreach (var hostDomainContextsStorage in _domainContextRegistrar.Values)
                    {
                        hostDomainContextsStorage.ReadonlyDomainContext.Dispose();
                        foreach (var domainContext in hostDomainContextsStorage.ModifiableDomainContexts.Values)
                        {
                            domainContext.Dispose();
                        }
                    }

                    _domainContextRegistrar.Clear();
                }
            }

            _isDisposed = true;
        }

        private class HostDomainContextsStorage
        {
            private readonly IDictionary<Type, IModifiableDomainContext> _modifiableDomainContexts = new Dictionary<Type, IModifiableDomainContext>();

            public HostDomainContextsStorage(IReadDomainContext readDomainContext)
            {
                ReadonlyDomainContext = readDomainContext;
            }

            public IReadDomainContext ReadonlyDomainContext { get; private set; }
            public IDictionary<Type, IModifiableDomainContext> ModifiableDomainContexts
            {
                get { return _modifiableDomainContexts; }
            }
        }
    }
}