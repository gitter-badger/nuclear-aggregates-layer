using System;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Simplified;

using Microsoft.Practices.Unity;

using NuClear.Aggregates.Storage.DI.Unity;
using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    internal sealed class UnityRuntimeFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDomainContextHost _domainContextHost;
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;

        public UnityRuntimeFactory(
            IUnityContainer unityContainer,
            IDomainContextHost domainContextHost, 
            ScopedDomainContextsStore scopedDomainContextsStore)
        {
            _unityContainer = unityContainer;
            _domainContextHost = domainContextHost;
            _scopedDomainContextsStore = scopedDomainContextsStore;
        }

        public ISimplifiedModelConsumer CreateConsumer(Type consumerType)
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

            var readDomainContextProvider = new ReadableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);
            var modifiableDomainContextProvider = new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);

            var dependencyOverrides = new DependencyOverrides
                {
                    // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                    // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                    // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                    { typeof(IReadableDomainContextProvider), readDomainContextProvider },
                    { typeof(IModifiableDomainContextProvider), modifiableDomainContextProvider }
                };

            return (ISimplifiedModelConsumer)_unityContainer.Resolve(consumerType, dependencyOverrides);
        }

        public ISimplifiedModelConsumer CreateConsumerReadModel(Type readModelType)
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

            var readDomainContextProvider = new ReadableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);

            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadableDomainContextProvider), readDomainContextProvider }
                        };

            return (ISimplifiedModelConsumer)_unityContainer.Resolve(readModelType, dependencyOverrides);
        }

        public IPersistenceService CreatePersistenceService(Type persistenceServiceType)
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

            if (persistenceServiceType.IsInterface)
            {
                throw new InvalidOperationException("Can't create persistence service instance by interface type: " + persistenceServiceType);
            }

            var readDomainContextProvider = new ReadableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);
            var modifiableDomainContextProvider = new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);

            var dependencyOverrides = new DependencyOverrides
                {
                    // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                    // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                    // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                    { typeof(IReadableDomainContextProvider), readDomainContextProvider },
                    { typeof(IModifiableDomainContextProvider), modifiableDomainContextProvider }
                };

            return (IPersistenceService)_unityContainer.Resolve(persistenceServiceType, dependencyOverrides);
        }
    }
}
