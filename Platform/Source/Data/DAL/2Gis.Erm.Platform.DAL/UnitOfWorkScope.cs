using System;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Aggregates;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// область внутри бизнес операции, контекст участка бизнес-операции. Время жизни меньше, чем у UnitOfWork. Может быть несколько экзепляров UnitOfWorkScope для одного UnitOfWork, как независимых, так и вложенных. 
    /// После окончания жизни UnitOfWorkScope удаляются DomainContext’ы, созданные внутри него. В случае вложенных UnitOfWorkScope, должен быть явно создан TransactionScope.
    /// UnitOfWorkScope имеет метод Complete, который транзакционно вызывает методы сохранения DomainContext’ов
    /// </summary>
    public sealed class UnitOfWorkScope : IUnitOfWorkScope, IPendingChangesMonitorable
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

        #region Implementation of IDomainContextHost

        Guid IDomainContextHost.ScopeId
        {
            get { return _id; }
        }

        #endregion

        #region Implementation of IPendingChangesMonitorable

        bool IPendingChangesMonitorable.AnyPendingChanges
        {
            get { return _anyPendingChanges; }
        }

        #endregion

        public int Complete()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("Object was already disposed");
            }
            var count = 0;
            _anyPendingChanges = false;

            var contexts = _unitOfWork.GetModifiableDomainContexts(this);
            if (contexts == null || !contexts.Any())
            {
                return count;
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                foreach (IModifiableDomainContext context in contexts)
                {
                    count += context.SaveChanges(SaveOptions.None);
                }

                scope.Complete();

                foreach (IModifiableDomainContext context in contexts)
                {
                    context.AcceptAllChanges();
                }
            }
            
            return count;
        }

        #region Implementation of ISpecificRepositoryFactory

        public TAggregateRepository CreateRepository<TAggregateRepository>() where TAggregateRepository : class, IAggregateRepository
        {
            var targetType = typeof(TAggregateRepository);
            if (!targetType.IsInterface)
            {
                throw new InvalidOperationException("Can't create aggregate repository as concrete type " + targetType + " you must use it through interface");
            }

            return _aggregateRepositoryForHostFactory.CreateRepository<TAggregateRepository>(this);
        }

        #endregion

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                _pendingChangesHandlingStrategy.HandlePendingChanges(this);

                var modifiableDomainContexts = _unitOfWork.DeattachModifiableDomainContexts(this);
                if (modifiableDomainContexts != null)
                {
                    foreach (var domainContext in modifiableDomainContexts)
                    {
                        domainContext.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        #endregion
    }
}