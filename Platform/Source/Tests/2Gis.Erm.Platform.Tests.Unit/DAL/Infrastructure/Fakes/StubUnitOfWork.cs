using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubUnitOfWork : UnitOfWork
    {
        private readonly IDictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>> _factories;

        public StubUnitOfWork(
            IDictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>> factories,
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ICommonLog logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
            _factories = factories;
        }

        protected override object CreateRepository(Type aggregateRepositoryType,
                                                   IReadDomainContextProvider readDomainContextProvider,
                                                   IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository> factory;
            if (!_factories.TryGetValue(aggregateRepositoryType, out factory))
            {
                throw new NotSupportedException("Specified type of aggregate repository is not supported, because factory method is not defined");
            }

            return factory(readDomainContextProvider, modifiableDomainContextProvider);
        }

        protected override object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            throw new NotSupportedException();
        }

        protected override object CreateConsumerReadModel(Type readModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreateAggregateReadModel(Type aggregateReadModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotSupportedException();
        }

        protected override object CreatePersistenceService(Type persistenceServiceType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            throw new NotSupportedException();
        }
    }
}
