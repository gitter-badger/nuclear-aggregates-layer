using System;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class MockUnitOfWork : UnitOfWork
    {
        private readonly Action<IReadDomainContextProvider, IModifiableDomainContextProvider, IDomainContextSaveStrategy> _createRepositoryAction;

        public MockUnitOfWork(
            Action<IReadDomainContextProvider, IModifiableDomainContextProvider, IDomainContextSaveStrategy> createRepositoryAction,
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ICommonLog logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
            _createRepositoryAction = createRepositoryAction;
        }

        public MockUnitOfWork(
            IReadDomainContext readDomainContext,
            IModifiableDomainContextFactory modifiableDomainContextFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
            ICommonLog logger)
            : base(readDomainContext, modifiableDomainContextFactory, pendingChangesHandlingStrategy, logger)
        {
        }

        public bool IsDisposedPublic
        {
            get { return IsDisposed; }
        }

        protected override object CreateRepository(Type aggregateRepositoryType,
                                                   bool createByConcreteType,
                                                   IReadDomainContextProvider readDomainContextProvider,
                                                   IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                   IDomainContextSaveStrategy saveStrategy)
        {
            _createRepositoryAction(readDomainContextProvider, modifiableDomainContextProvider, saveStrategy);
            return null;
        }

        protected override object CreateAggregateReadModel(Type aggregateReadModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreateConsumer(Type consumerType,
                                                 IReadDomainContextProvider readDomainContextProvider,
                                                 IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                 IDomainContextSaveStrategy saveStrategy)
        {
            throw new NotImplementedException();
        }

        protected override object CreateCosumerReadModel(Type readModelType, IReadDomainContextProvider readDomainContextProvider)
        {
            throw new NotImplementedException();
        }

        protected override object CreatePersistenceService(Type persistenceServiceType,
                                                           IReadDomainContextProvider readDomainContextProvider,
                                                           IModifiableDomainContextProvider modifiableDomainContextProvider,
                                                           IDomainContextSaveStrategy saveStrategy)
        {
            throw new NotImplementedException();
        }
    }
}