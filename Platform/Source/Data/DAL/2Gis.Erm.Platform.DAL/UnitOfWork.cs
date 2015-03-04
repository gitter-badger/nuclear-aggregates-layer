using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Simplified;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.DAL
{
    public abstract partial class UnitOfWork : IUnitOfWork, 
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
        private readonly ITracer _logger;

        private readonly object _domainContextRegistrarSynch = new object();
        private readonly IDictionary<Guid, HostDomainContextsStorage> _domainContextRegistrar = new Dictionary<Guid, HostDomainContextsStorage>();

        private readonly Guid _directlyNestedDomainContextsHostId = Guid.NewGuid();

        protected UnitOfWork(IReadDomainContext readDomainContext,
                             IModifiableDomainContextFactory modifiableDomainContextFactory,
                             IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                             ITracer logger)
        {
            _readDomainContext = readDomainContext;
            _modifiableDomainContextFactory = modifiableDomainContextFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _logger = logger;
        }

        Guid IDomainContextHost.ScopeId
        {
            get { return _directlyNestedDomainContextsHostId; }
        }

        public TAggregateRepository CreateRepository<TAggregateRepository>() where TAggregateRepository : class, IAggregateRepository
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return (TAggregateRepository)CreateRepository(targetType, this);
        }

        TAggregateRepository IAggregateRepositoryForHostFactory.CreateRepository<TAggregateRepository>(IDomainContextHost domainContextHost)
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return (TAggregateRepository)CreateRepository(targetType, domainContextHost);
        }

        object IAggregatesLayerRuntimeFactory.CreateRepository(Type aggregateRepositoryType)
        {
            if (!aggregateRepositoryType.IsAggregateRepository())
            {
                throw new ArgumentException("Type specified as aggregate repository have to implement interace " + ModelIndicators.Aggregates.Repository);
            }

            if (aggregateRepositoryType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository by interface " + aggregateRepositoryType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateRepository(aggregateRepositoryType, this);
        }

        object IAggregatesLayerRuntimeFactory.CreateAggregateReadModel(Type aggregateReadModelType)
        {
            if (!aggregateReadModelType.IsAggregateReadModel())
            {
                throw new ArgumentException("Type specified as aggregate read model have to implement interace " + ModelIndicators.Aggregates.ReadModel);
            }

            if (aggregateReadModelType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate read model by interface " + aggregateReadModelType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(this, this);
            return CreateAggregateReadModel(aggregateReadModelType, readDomainContextProviderProxy);
        }

        object ISimplifiedModelConsumerRuntimeFactory.CreateConsumer(Type consumerType)
        {
            if (!consumerType.IsSimplifiedModelConsumer())
            {
                throw new ArgumentException("Type specified as simplified model consumer have to implement interace " + typeof(ISimplifiedModelConsumer).Name);
            }

            if (consumerType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer by interface " + consumerType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateConsumer(consumerType,
                                  new ReadDomainContextProviderProxy(this, this),
                                  new ModifiableDomainContextProviderProxy(this, this));
        }

        object ISimplifiedModelConsumerRuntimeFactory.CreateAggregateReadModel(Type readModelType)
        {
            if (!readModelType.IsSimplifiedModelConsumerReadModel())
            {
                throw new ArgumentException("Type specified as simplified model consumer have to implement interace " + typeof(ISimplifiedModelConsumer).Name);
            }

            if (readModelType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer read model by interface " + readModelType + ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateConsumer(readModelType,
                                  new ReadDomainContextProviderProxy(this, this),
                                  new ModifiableDomainContextProviderProxy(this, this));
        }

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
                new ModifiableDomainContextProviderProxy(this, this));
        }

        /// <summary>
        /// Возвращает для указанного host все связанные с ним domain context, допускающие модификацию данных
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        IEnumerable<IModifiableDomainContext> IUnitOfWork.GetModifiableDomainContexts(IDomainContextHost host)
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
        IEnumerable<IModifiableDomainContext> IUnitOfWork.DeattachModifiableDomainContexts(IDomainContextHost host)
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
        IUnitOfWorkScope IUnitOfWork.CreateScope()
        {
            return new UnitOfWorkScope(this, this, _pendingChangesHandlingStrategy);
        }

        IReadDomainContext IReadDomainContextProviderForHost.Get(IDomainContextHost domainContextHost)
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

        protected abstract object CreateRepository(Type aggregateRepositoryType,
                                                   IReadDomainContextProvider readDomainContextProvider,
                                                   IModifiableDomainContextProvider modifiableDomainContextProvider);

        protected abstract object CreateAggregateReadModel(Type aggregateReadModelType,
                                                           IReadDomainContextProvider readDomainContextProvider);

        protected abstract object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider);

        protected abstract object CreateCosumerReadModel(Type readModelType,
                                                          IReadDomainContextProvider readDomainContextProvider);

        protected abstract object CreatePersistenceService(Type consumerType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider);

        private object CreateRepository(Type aggregateRepositoryType, IDomainContextHost domainContextHost)
        {
            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(this, domainContextHost);
            var modifiableDomainContextProviderProxy = new ModifiableDomainContextProviderProxy(this, domainContextHost);
            return CreateRepository(
                aggregateRepositoryType,
                readDomainContextProviderProxy,
                modifiableDomainContextProviderProxy);
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