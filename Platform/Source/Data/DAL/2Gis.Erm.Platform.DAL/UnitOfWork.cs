using System;

using DoubleGis.Erm.Platform.DAL.Model.SimplifiedModel;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Simplified;

using NuClear.Aggregates.Storage;
using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.DAL
{
    public abstract partial class UnitOfWork : IUnitOfWork,
                                               ISimplifiedModelConsumerRuntimeFactory,
                                               IPersistenceServiceRuntimeFactory
    {
        private readonly IDomainContextHost _domainContextHost;
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly ScopedAggregateRepositoryFactory _scopedAggregateRepositoryFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        protected UnitOfWork(
            IDomainContextHost domainContextHost,
            ScopedDomainContextsStore scopedDomainContextsStore,
            ScopedAggregateRepositoryFactory scopedAggregateRepositoryFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            _domainContextHost = domainContextHost;
            _scopedDomainContextsStore = scopedDomainContextsStore;
            _scopedAggregateRepositoryFactory = scopedAggregateRepositoryFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
        }

        object ISimplifiedModelConsumerRuntimeFactory.CreateConsumer(Type consumerType)
        {
            if (!consumerType.IsSimplifiedModelConsumer())
            {
                throw new ArgumentException("Type specified as simplified model consumer have to implement interace " + typeof(ISimplifiedModelConsumer).Name);
            }

            if (consumerType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer by interface " + consumerType +
                                                    ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateConsumer(consumerType,
                                  new ReadDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost),
                                  new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost));
        }

        object ISimplifiedModelConsumerRuntimeFactory.CreateAggregateReadModel(Type readModelType)
        {
            if (!readModelType.IsSimplifiedModelConsumerReadModel())
            {
                throw new ArgumentException("Type specified as simplified model consumer have to implement interace " + typeof(ISimplifiedModelConsumer).Name);
            }

            if (readModelType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer read model by interface " + readModelType +
                                                    ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreateConsumer(readModelType,
                                  new ReadDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost),
                                  new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost));
        }

        object IPersistenceServiceRuntimeFactory.CreatePersistenceService(Type persistenceServiceType)
        {
            if (!typeof(IPersistenceService).IsAssignableFrom(persistenceServiceType))
            {
                throw new ArgumentException("Type specified as persistence service have to implement interace " + typeof(IPersistenceService).Name);
            }

            if (persistenceServiceType.IsInterface)
            {
                throw new InvalidOperationException("Can't create persistence service by interface " + persistenceServiceType +
                                                    ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            return CreatePersistenceService(persistenceServiceType,
                                            new ReadDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost),
                                            new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost));
        }

        /// <summary>
        /// Создает новый UoWScope
        /// </summary>
        /// <returns></returns>
        IUnitOfWorkScope IUnitOfWork.CreateScope()
        {
            return new UnitOfWorkScope(_scopedDomainContextsStore, _scopedAggregateRepositoryFactory, _pendingChangesHandlingStrategy);
        }

        protected abstract object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider);

        protected abstract object CreateConsumerReadModel(Type readModelType,
                                                          IReadDomainContextProvider readDomainContextProvider);

        protected abstract object CreatePersistenceService(Type consumerType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider);
    }
}