using System;
using System.Reflection;

using Microsoft.Practices.Unity;

using NuClear.Aggregates;
using NuClear.Storage.Core;

namespace Aggregates.Storage.DI.Unity
{
    internal class UnityAggregateReadModelFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDomainContextHost _domainContextHost;
        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;

        public UnityAggregateReadModelFactory(
            IUnityContainer unityContainer,
            IDomainContextHost domainContextHost, 
            ScopedDomainContextsStore scopedDomainContextsStore)
        {
            _unityContainer = unityContainer;
            _domainContextHost = domainContextHost;
            _scopedDomainContextsStore = scopedDomainContextsStore;
        }

        public IAggregateReadModel CreateAggregateReadModel(Type aggregateReadModelType)
        {
            if (!aggregateReadModelType.IsAggregateReadModel())
            {
                throw new ArgumentException("Type specified as aggregate read model have to implement interace " + Indicators.Aggregates.ReadModel);
            }

            if (aggregateReadModelType.GetTypeInfo().IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate read model by interface " + aggregateReadModelType +
                                                    ". Factory must be used for concrete types only. Try check and use mapping interface2concrete");
            }
            
            var readDomainContextProviderProxy = new ReadDomainContextProviderProxy(_scopedDomainContextsStore, _domainContextHost);

            var dependencyOverrides = new DependencyOverrides
                {
                    // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                    // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                    // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                    { typeof(IReadDomainContextProvider), readDomainContextProviderProxy },
                };

            return (IAggregateReadModel)_unityContainer.Resolve(aggregateReadModelType, Mapping.ExplicitlyCreatedAggregateRepositoriesScope, dependencyOverrides);
        }
    }
}