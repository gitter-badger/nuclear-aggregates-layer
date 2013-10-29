using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubUnitOfWork : UnitOfWork
    {
        private readonly IDictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IDomainContextSaveStrategy, IAggregateRepository>>
            _factories;

        public StubUnitOfWork(
            IDictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IDomainContextSaveStrategy, IAggregateRepository>> factories,
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ICommonLog logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
            _factories = factories;
        }

        #region Overrides of UnitOfWork

        protected override object CreateRepository(Type aggregateRepositoryType,
                                                   bool createByConcreteType,
                                                   IReadDomainContextProvider readDomainContextProvider,
                                                   IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                   IDomainContextSaveStrategy saveStrategy)
        {
            Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IDomainContextSaveStrategy, IAggregateRepository> factory;
            if (!_factories.TryGetValue(aggregateRepositoryType, out factory))
            {
                throw new NotSupportedException("Specified type of aggregate repository is not supported, because factory method is not defined");
            }

            return factory(readDomainContextProvider, modifiableDomainContextProvider, saveStrategy);
        }

        protected override object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                 IDomainContextSaveStrategy saveStrategy)
        {
            throw new NotSupportedException();
        }

        protected override object CreateReadModel(Type aggregateReadModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotSupportedException();
        }

        protected override object CreatePersistenceService(Type persistenceServiceType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                           IDomainContextSaveStrategy saveStrategy)
        
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}

