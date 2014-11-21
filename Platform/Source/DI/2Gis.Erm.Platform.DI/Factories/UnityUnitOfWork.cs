using System;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.Factories
{
    public sealed class UnityUnitOfWork : UnitOfWork
    {
        private readonly IUnityContainer _unityContainer;

        public UnityUnitOfWork(
            IUnityContainer unityContainer, 
            IReadDomainContext readDomainContext, 
            IModifiableDomainContextFactory modifiableDomainContextFactory, 
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ICommonLog logger)
        : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
            _unityContainer = unityContainer;
        }

        #region Overrides of UnitOfWork

        protected override object CreateRepository(
            Type aggregateRepositoryType,
            bool createByConcreteType,
            IReadDomainContextProvider readDomainContextProvider, 
            IModifiableDomainContextProvider modifiableDomainContextProvider, 
            IDomainContextSaveStrategy saveStrategy)
        {
            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadDomainContextProvider), readDomainContextProvider },
                            { typeof(IModifiableDomainContextProvider), modifiableDomainContextProvider },
                            { typeof(IDomainContextSaveStrategy), saveStrategy }
                        };

            // FIXME {all, 23.04.2014}: legacy, теперь нет разницы запрашиваем создание через абстракцию или конкретный тип - нужно выкосить createByConcreteType + не поломать unit tests 

            return createByConcreteType
                       ? _unityContainer.Resolve(aggregateRepositoryType, Mapping.ExplicitlyCreatedAggregateRepositoriesScope, dependencyOverrides) // запросили создание конкретного типа - никакие scope не используем, 
                       : _unityContainer.Resolve(aggregateRepositoryType, Mapping.ExplicitlyCreatedAggregateRepositoriesScope, dependencyOverrides); // запросили создание, указав интерфейс, направляем resolve в специальный scope
        }

        protected override object CreateAggregateReadModel(Type aggregateReadModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadDomainContextProvider), readDomainContextProvider }
                        };

            return _unityContainer.Resolve(aggregateReadModelType, dependencyOverrides);
        }

        protected override object CreateConsumer(Type consumerType, IReadDomainContextProvider readDomainContextProvider, IModifiableDomainContextProvider modifiableDomainContextProvider, IDomainContextSaveStrategy saveStrategy)
        {
            if (consumerType.IsInterface)
            {
                throw new InvalidOperationException("Can't create simplified model consumer instance by interface type: " + consumerType);
            }

            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadDomainContextProvider), readDomainContextProvider },
                            { typeof(IModifiableDomainContextProvider), modifiableDomainContextProvider },
                            { typeof(IDomainContextSaveStrategy), saveStrategy }
                        };

            return _unityContainer.Resolve(consumerType, dependencyOverrides);
        }

        protected override object CreateCosumerReadModel(Type readModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadDomainContextProvider), readDomainContextProvider }
                        };

            return _unityContainer.Resolve(readModelType, dependencyOverrides);
        }

        protected override object CreatePersistenceService(Type persistenceServiceType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                           IDomainContextSaveStrategy saveStrategy)
        {
            if (persistenceServiceType.IsInterface)
            {
                throw new InvalidOperationException("Can't create persistence service instance by interface type: " + persistenceServiceType);
            }

            var dependencyOverrides = new DependencyOverrides
                        {
                            // указываем какие экземпляры использовать при resolve нижеуказанных зависимостей
                            // данные типы зависимостей даже не должны регистророваться в DI-контейнере, т.е. resolve
                            // работает ТОЛЬКО из-за того, что мы явно указываем какие экземпляры для каких типов зависимостей нужно использовать
                            { typeof(IReadDomainContextProvider), readDomainContextProvider },
                            { typeof(IModifiableDomainContextProvider), modifiableDomainContextProvider },
                            { typeof(IDomainContextSaveStrategy), saveStrategy }
                        };

            return _unityContainer.Resolve(persistenceServiceType, dependencyOverrides);
        }

        #endregion
    }
}
