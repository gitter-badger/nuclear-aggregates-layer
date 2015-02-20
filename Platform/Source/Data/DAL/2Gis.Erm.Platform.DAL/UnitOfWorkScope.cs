using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Model.Aggregates;

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

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAggregateRepositoryForHostFactory _aggregateRepositoryForHostFactory;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        
        private bool _anyPendingChanges = true;

        internal UnitOfWorkScope(
            IUnitOfWork unitOfWork, 
            IAggregateRepositoryForHostFactory aggregateRepositoryForHostFactory,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (aggregateRepositoryForHostFactory == null)
            {
                throw new ArgumentNullException("aggregateRepositoryForHostFactory");
            }

            _unitOfWork = unitOfWork;
            _aggregateRepositoryForHostFactory = aggregateRepositoryForHostFactory;
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

            var contexts = _unitOfWork.GetModifiableDomainContexts(this);
            if (contexts == null 
                || contexts.All(c => !c.AnyPendingChanges))
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

            return _aggregateRepositoryForHostFactory.CreateRepository<TAggregateRepository>(this);
        }
    }
}