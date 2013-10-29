using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.DAL
{
    public abstract class UnitOfWork : IUnitOfWork, 
                                       IDomainContextHost,
                                       IAggregateRepositoryForHostFactory,
                                       IAggregatesLayerRuntimeFactory,
                                       ISimplifiedModelConsumerRuntimeFactory,
                                       IPersistenceServiceRuntimeFactory,
                                       IReadDomainContextProviderForHost, 
                                       IModifiableDomainContextProviderForHost
    {
        private readonly IReadDomainContext _readDomainContext;

        // фабрики, для реального создания экземпляров domaincontext
        private readonly IModifiableDomainContextFactory _modifiableDomainContextFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly ICommonLog _logger;

        #region Хранилище Domain Context которые контролируются UoW

        private readonly object _domainContextRegistrarSynch = new object();
        private readonly IDictionary<Guid, HostDomainContextsStorage> _domainContextRegistrar = new Dictionary<Guid, HostDomainContextsStorage>();
        
        #endregion

        private readonly Guid _directlyNestedDomainContextsHostId = Guid.NewGuid();

        protected UnitOfWork(IReadDomainContext readDomainContext,
                             IModifiableDomainContextFactory modifiableDomainContextFactory,
                             IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                             ICommonLog logger)
        {
            _readDomainContext = readDomainContext;
            _modifiableDomainContextFactory = modifiableDomainContextFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _logger = logger;
        }

        #region Implementation of IDomainContextHost

        public Guid ScopeId
        {
            get { return _directlyNestedDomainContextsHostId; }
        }

        #endregion

        #region Implementation of IAggregateRepository factory functionality

        public TAggregateRepository CreateRepository<TAggregateRepository>() where TAggregateRepository : class, IAggregateRepository
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return (TAggregateRepository)CreateRepository(targetType, false, this, new DomainContextSaveStrategy(false));
        }

        TAggregateRepository IAggregateRepositoryForHostFactory.CreateRepository<TAggregateRepository>(IDomainContextHost domainContextHost)
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return (TAggregateRepository)CreateRepository(targetType, false, domainContextHost, new DomainContextSaveStrategy(true));
        }

        protected abstract object CreateRepository(
                                    Type aggregateRepositoryType,
                                    bool createByConcreteType,
                                    IReadDomainContextProvider readDomainContextProvider, 
                                    IModifiableDomainContextProvider modifiableDomainContextProvider, 
                                    IDomainContextSaveStrategy saveStrategy);

        protected abstract object CreateReadModel(
                                    Type aggregateReadModelType,
                                    IReadDomainContextProvider readDomainContextProvider);
        
        protected abstract object CreateConsumer(
                                    Type consumerType,
                                    IReadDomainContextProvider readDomainContextProvider,
                                    IModifiableDomainContextProvider modifiableDomainContextProvider,
                                    IDomainContextSaveStrategy saveStrategy);

        protected abstract object CreatePersistenceService(
                                    Type consumerType,
                                    IReadDomainContextProvider readDomainContextProvider,
                                    IModifiableDomainContextProvider modifiableDomainContextProvider,
                                    IDomainContextSaveStrategy saveStrategy);

        #endregion
        
        #region Implementation of IAggregatesLayerRuntimeFactory

        object IAggregatesLayerRuntimeFactory.CreateRepository(Type aggregateRepositoryType)
        {
            if (!ModelIndicators.IsAggregateRepository(aggregateRepositoryType))
            {
                throw new ArgumentException("Type specified as aggregate repository have to implement interace " + ModelIndicators.Aggregates.Repository);
            }

            if (aggregateRepositoryType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository by interface " + aggregateRepositoryType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateRepository(aggregateRepositoryType, true, this, new DomainContextSaveStrategy(false));
        }

        object IAggregatesLayerRuntimeFactory.CreateReadModel(Type aggregateReadModelType)
        {
            if (!ModelIndicators.IsReadModel(aggregateReadModelType))
            {
                throw new ArgumentException("Type specified as aggregate read model have to implement interace " + ModelIndicators.Aggregates.ReadModel);
            }

            if (aggregateReadModelType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate read model by interface " + aggregateReadModelType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(this, this);
            return CreateReadModel(aggregateReadModelType, readDomainContextProviderProxy);
        }

        #endregion

        #region Implementation of ISimplifiedModelConsumerFactory

        object ISimplifiedModelConsumerRuntimeFactory.CreateConsumer(Type consumerType)
        {
            if (!ModelIndicators.IsSimplifiedModelConsumer(consumerType))
            {
                throw new ArgumentException("Type specified as simplified model consumer have to implement interace " + typeof(ISimplifiedModelConsumer).Name);
            }

            if (consumerType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer by interface " + consumerType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateConsumer(consumerType,
                                  new ReadDomainContextProviderProxy(this, this),
                                  new ModifiableDomainContextProviderProxy(this, this),
                                  new DomainContextSaveStrategy(false));
        }

        #endregion

        #region Implementation of IPersistenceServiceRuntimeFactory

        object IPersistenceServiceRuntimeFactory.CreatePersistenceService(Type persistenceServiceType)
        {
            if (!typeof(IPersistenceService).IsAssignableFrom(persistenceServiceType))
            {
                throw new ArgumentException("Type specified as persistence service have to implement interace " + typeof(IPersistenceService).Name);
            }

            if (persistenceServiceType.IsInterface)
            {
                throw new InvalidOperationException("Can't create persistence service by interface " + persistenceServiceType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreatePersistenceService(persistenceServiceType,
                new ReadDomainContextProviderProxy(this, this),
                new ModifiableDomainContextProviderProxy(this, this),
                new DomainContextSaveStrategy(false));
        }

        #endregion

        #region Implementation of IUnitOfWork
        /// <summary>
        /// Возвращает для указанного host все связанные с ним domain context, допускающие модификацию данных
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public IEnumerable<IModifiableDomainContext> GetModifiableDomainContexts(IDomainContextHost host)
        {
            lock (_domainContextRegistrarSynch)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (_domainContextRegistrar.TryGetValue(host.ScopeId, out hostDomainContextsStorage))
                {
                    return hostDomainContextsStorage.ModifiableDomainContexts.Values.ToArray();
                }

                return Enumerable.Empty<IModifiableDomainContext>();
            }
        }

        /// <summary>
        /// Забирает из под контроля UoW все domain contexts, связанные с указанным host.
        /// и возвращает их в качестве результата.
        /// Ответственность за вызов dispose для полученных через результат domain contexts лежит на вызывающем коде
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public IEnumerable<IModifiableDomainContext> DeattachModifiableDomainContexts(IDomainContextHost host)
        {
            lock (_domainContextRegistrarSynch)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (_domainContextRegistrar.TryGetValue(host.ScopeId, out hostDomainContextsStorage))
                {
                    _domainContextRegistrar.Remove(host.ScopeId);
                    return hostDomainContextsStorage.ModifiableDomainContexts.Values;
                }

                return Enumerable.Empty<IModifiableDomainContext>();
            }
        }

        /// <summary>
        /// Создает новый UoWScope
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkScope CreateScope()
        {
            return new UnitOfWorkScope(this, this, _pendingChangesHandlingStrategy);
        }

        #endregion

        #region Implementation of IReadDomainContextProviderForHost

        public IReadDomainContext Get(IDomainContextHost domainContextHost)
        {
            lock (_domainContextRegistrarSynch)
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

        #endregion
        
        #region Implementation of IModifiableDomainContextProviderForHost

        IModifiableDomainContext IModifiableDomainContextProviderForHost.Get<TEntity>(IDomainContextHost domainContextHost)
        {
            Type targetEntityType = typeof(TEntity); // важно т.к. domaincontext привязываются к (соответствуют) сущностным репозиториям
            IModifiableDomainContext domainContext;

            lock (_domainContextRegistrarSynch)
            {
                HostDomainContextsStorage hostDomainContextsStorage;
                if (!_domainContextRegistrar.TryGetValue(domainContextHost.ScopeId, out hostDomainContextsStorage))
                {
                    hostDomainContextsStorage = new HostDomainContextsStorage(_readDomainContext);
                    _domainContextRegistrar.Add(domainContextHost.ScopeId, hostDomainContextsStorage);
                }

                var modifiableDomainContexts = hostDomainContextsStorage.ModifiableDomainContexts;
                if (!modifiableDomainContexts.TryGetValue(targetEntityType, out domainContext))
                {   // контекста нет, нужно создать
                    domainContext = _modifiableDomainContextFactory.Create<TEntity>();
                    modifiableDomainContexts.Add(targetEntityType, domainContext);
                }
            }

            return domainContext;
        }

        #endregion

        private object CreateRepository(Type aggregateRepositoryType, bool createByConcreteType, IDomainContextHost domainContextHost, IDomainContextSaveStrategy saveStrategy)
        {
            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(this, domainContextHost);
            var modifiableDomainContextProviderProxy = new ModifiableDomainContextProviderProxy(this, domainContextHost);
            return CreateRepository(
                aggregateRepositoryType,
                createByConcreteType,
                readDomainContextProviderProxy,
                modifiableDomainContextProviderProxy,
                saveStrategy);
        }

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный + для подклассов
        /// </summary>
        protected bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Внутренний dispose самого базового класса
        /// </summary>
        public void Dispose()
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                // сначала вызываем реализацию у потомков
                OnDispose();

                // теперь отрабатывает сам базовый класс
                lock (_domainContextRegistrarSynch)
                {
                    HostDomainContextsStorage directlyNestedDomainContexts;
                    if (_domainContextRegistrar.TryGetValue(_directlyNestedDomainContextsHostId, out directlyNestedDomainContexts))
                    {
                        directlyNestedDomainContexts.ReadonlyDomainContext.Dispose();
                        foreach (var domainContext in directlyNestedDomainContexts.ModifiableDomainContexts.Values)
                        {
                            domainContext.Dispose();
                        }

                        _domainContextRegistrar.Remove(_directlyNestedDomainContextsHostId);
                    }

                    if (_domainContextRegistrar.Count > 0)
                    {
                        {
                            // Логирование
                            var directlyNestedDomainContextRepresentation = directlyNestedDomainContexts != null
                                ? string.Join(", ", directlyNestedDomainContexts.ModifiableDomainContexts.Keys)
                                : string.Empty;

                            var domainContextTextRepresentation = string.Join("\n", _domainContextRegistrar.Values.Select(x => string.Join(", ", x.ModifiableDomainContexts.Keys)));

                            _logger.ErrorEx(string.Format("При завершении UoW, обнаружены неочищенные domaincontext от каких-то domain context host - скорее всего где-то не вызвали dispose у UoWScope\nDirectly nested DC: {0}\nRemaining DCs: \n{1}",
                                directlyNestedDomainContextRepresentation,
                                domainContextTextRepresentation));
                        }

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
        }

        /// <summary>
        /// Обработчик dispose для подклассов
        /// </summary>
        protected virtual void OnDispose()
        {
        }

        #endregion

        private class HostDomainContextsStorage
        {
            private readonly IDictionary<Type, IModifiableDomainContext> _modifiableDomainContexts = new Dictionary<Type, IModifiableDomainContext>();

            public IReadDomainContext ReadonlyDomainContext { get; private set; }
            public IDictionary<Type, IModifiableDomainContext> ModifiableDomainContexts
            {
                get { return _modifiableDomainContexts; }
            }

            public HostDomainContextsStorage(IReadDomainContext readDomainContext)
            {
                ReadonlyDomainContext = readDomainContext;
            }
        }
    }
}