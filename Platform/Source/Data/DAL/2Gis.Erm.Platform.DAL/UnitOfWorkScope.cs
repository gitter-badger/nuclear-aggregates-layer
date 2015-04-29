using System;

using NuClear.Aggregates.Storage;
using NuClear.Storage.Core;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// область внутри бизнес операции, контекст участка бизнес-операции. Время жизни меньше, чем у UnitOfWork. 
    /// Может быть несколько экзепляров UnitOfWorkScope для одного UnitOfWork, как независимых, так и вложенных. 
    /// После окончания жизни UnitOfWorkScope удаляются DomainContext’ы, созданные внутри него.
    /// </summary>
    public sealed partial class UnitOfWorkScope : IUnitOfWorkScope, IPendingChangesMonitorable
    {
        private readonly Guid _id = Guid.NewGuid();

        private readonly ScopedDomainContextsStore _scopedDomainContextsStore;
        private readonly ScopedAggregateRepositoryFactory _scopedAggregateRepositoryFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        
        private bool _anyPendingChanges = true;

        internal UnitOfWorkScope(
            ScopedDomainContextsStore scopedDomainContextsStore,
            ScopedAggregateRepositoryFactory scopedAggregateRepositoryFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            if (scopedDomainContextsStore == null)
            {
                throw new ArgumentNullException("scopedDomainContextsStore");
            }

            if (scopedAggregateRepositoryFactory == null)
            {
                throw new ArgumentNullException("scopedAggregateRepositoryFactory");
            }

            _scopedDomainContextsStore = scopedDomainContextsStore;
            _scopedAggregateRepositoryFactory = scopedAggregateRepositoryFactory;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
        }

        Guid IDomainContextHost.ScopeId
        {
            get { return _id; }
        }

        bool IPendingChangesMonitorable.AnyPendingChanges
        {
            get { return _anyPendingChanges; }
        }

        void IUnitOfWorkScope.Complete()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("Object was already disposed");
            }

            if (_scopedDomainContextsStore.AnyPendingChanges(this))
            { 
                _anyPendingChanges = false;
                return;
            }

            throw new InvalidOperationException("Some of the domain contexts hosted by UoWScope has unsaved changes, when UoWScope completes - check aggregates layer implementations");
        }

        TAggregateRepository IAggregateRepositoryFactory.CreateRepository<TAggregateRepository>()
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return _scopedAggregateRepositoryFactory.CreateRepository<TAggregateRepository>(this);
        }
    }
}