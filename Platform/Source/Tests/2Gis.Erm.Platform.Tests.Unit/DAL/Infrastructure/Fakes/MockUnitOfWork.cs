using System;

using DoubleGis.Erm.Platform.DAL;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class MockUnitOfWork : UnitOfWork
    {
        private readonly Action<IReadDomainContextProvider, IModifiableDomainContextProvider> _createRepositoryAction;

        public MockUnitOfWork(
            Action<IReadDomainContextProvider, IModifiableDomainContextProvider> createRepositoryAction,
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ITracer logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
            _createRepositoryAction = createRepositoryAction;
        }

        public MockUnitOfWork(
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ITracer logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
        }

        public bool IsDisposedPublic
        {
            get { return IsDisposed; }
        }

        protected override object CreateRepository(Type aggregateRepositoryType,
                                                   IReadDomainContextProvider readDomainContextProvider,
                                                   IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            _createRepositoryAction(readDomainContextProvider, modifiableDomainContextProvider);
            return null;
        }

        protected override object CreateAggregateReadModel(Type aggregateReadModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreateCosumerReadModel(Type readModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreatePersistenceService(Type persistenceServiceType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            throw new NotImplementedException();
        }
    }
}