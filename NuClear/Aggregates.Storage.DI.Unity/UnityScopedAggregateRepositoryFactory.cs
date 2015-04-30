using System;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.Storage.Core;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    internal class UnityScopedAggregateRepositoryFactory 
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDomainContextHost _domainContextHost;
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;

        public UnityScopedAggregateRepositoryFactory(
            IUnityContainer unityContainer,
            IDomainContextHost domainContextHost,
            ScopedDomainContextsStore scopedDomainContextsStore)
        {
            _unityContainer = unityContainer;
            _domainContextHost = domainContextHost;
            _scopedDomainContextsStore = scopedDomainContextsStore;
        }

        public IAggregateRepository CreateRepository(Type aggregateRepositoryType)
        {
            if (!aggregateRepositoryType.IsAggregateRepository())
            {
                throw new ArgumentException("Type specified as aggregate repository have to implement interace " + Indicators.Aggregates.Repository);
            }

            if (aggregateRepositoryType.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository by interface " + aggregateRepositoryType +
                                                    ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }

            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);
            var modifiableDomainContextProviderProxy = new ModifiableDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);

            var dependencyOverrides = new DependencyOverrides
                {
                    // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                    // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                    // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                    { typeof(IReadDomainContextProvider), readDomainContextProviderProxy },
                    { typeof(IModifiableDomainContextProvider), modifiableDomainContextProviderProxy }
                };

            return (IAggregateRepository)_unityContainer.Resolve(aggregateRepositoryType, Mapping.ExplicitlyCreatedAggregateRepositoriesScope, dependencyOverrides);
        }
    }
}