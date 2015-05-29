using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.Transactions;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Operations.Logging
{
    public sealed class TransactedOperationScope : IOperationScope
    {
        private readonly Guid _scopeId;
        private readonly bool _isRootScope;
        private readonly StrictOperationIdentity _strictOperationIdentity;
        private readonly IOperationScopeContextsStorage _operationScopeContextsStorage;
        private readonly IOperationScopeLifetimeManager _operationScopeLifetimeManager;
        private readonly TransactionScope _transactionScope;

        private readonly object _sync = new object();
        private bool _isCompleted;

        public TransactedOperationScope(Guid scopeId,
                                        bool isRootScope,
                                        StrictOperationIdentity strictOperationIdentity,
                                        IOperationScopeContextsStorage operationScopeContextsStorage,
                                        IOperationScopeLifetimeManager operationScopeLifetimeManager)
        {
            _scopeId = scopeId;
            _isRootScope = isRootScope;
            _strictOperationIdentity = strictOperationIdentity;
            _operationScopeContextsStorage = operationScopeContextsStorage;
            _operationScopeLifetimeManager = operationScopeLifetimeManager;

            // COMMENT {all, 28.07.2014}: хотя инфраструктура operationscopes содержит базу для работы в режиме многопоточности (на уровне контрактов, использование callcontext и т.п.) - здесь используется transctionscope в самом простом виде,  при реальном начале использования многопоточных usecases необходимы доработки - dependent transaction и т.п.
            _transactionScope = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default);
        }

        public Guid Id
        {
            get { return _scopeId; }
        }

        public StrictOperationIdentity OperationIdentity
        {
            get { return _strictOperationIdentity; }
        }

        public bool IsRoot
        {
            get { return _isRootScope; }
        }

        public bool Completed
        {
            get
            {
                lock (_sync)
                {
                    return _isCompleted;
                }
            }
        }

        public IOperationScope Added<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Added<TEntity>(this, new[] { changedEntity }.Concat(changedEntities));
            return this;
        }

        public IOperationScope Added<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Added<TEntity>(this, changedEntities);
            return this;
        }

        public IOperationScope Deleted<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Deleted<TEntity>(this, new[] { changedEntity }.Concat(changedEntities));
            return this;
        }

        public IOperationScope Deleted<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Deleted<TEntity>(this, changedEntities);
            return this;
        }

        public IOperationScope Updated<TEntity>(long changedEntity, params long[] changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Updated<TEntity>(this, new[] { changedEntity }.Concat(changedEntities));
            return this;
        }

        public IOperationScope Updated<TEntity>(IEnumerable<long> changedEntities) where TEntity : class, IEntity
        {
            _operationScopeContextsStorage.Updated<TEntity>(this, changedEntities);
            return this;
        }

        public IOperationScope Complete()
        {
            lock (_sync)
            {
                _isCompleted = true;
            }

            return this;
        }

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        public bool IsDisposed
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
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose самого базового класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                try
                {
                    _operationScopeLifetimeManager.Close(this);
                    if (Completed)
                    {
                        _transactionScope.Complete();
                    }
                }
                finally
                {
                    _transactionScope.Dispose();
                }

                if (disposing)
                {
                    // Free other state (managed objects).
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _isDisposed = true;
            }
        }

        #endregion
    }
}