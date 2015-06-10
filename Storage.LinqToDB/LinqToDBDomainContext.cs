using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Storage.Core;

namespace NuClear.Storage.LinqToDB
{
    public class LinqToDBDomainContext : IModifiableDomainContext, IReadableDomainContext
    {
        private readonly HashSet<object> _added = new HashSet<object>();
        private readonly HashSet<object> _updated = new HashSet<object>();
        private readonly HashSet<object> _deleted = new HashSet<object>();

        private readonly DataConnection _dataConnection;
        private readonly TransactionOptions _transactionOptions;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        private readonly MethodInfo _genericGetTableMethod = typeof(DataConnection).GetMethod("GetTable");
        
        public LinqToDBDomainContext(
            DataConnection dataConnection, 
            TransactionOptions transactionOptions,
            IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            _dataConnection = dataConnection;
            _transactionOptions = transactionOptions;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
        }

        bool IPendingChangesMonitorable.AnyPendingChanges
        {
            get { return _added.Count > 0 || _updated.Count > 0 || _deleted.Count > 0; }
        }

        IQueryable IReadableDomainContext.GetQueryableSource(Type entityType)
        {
            var methodInfo = _genericGetTableMethod.MakeGenericMethod(entityType);
            var lambda = Expression.Lambda<Func<IQueryable>>(Expression.Call(methodInfo, Expression.Constant(_dataConnection)));
            return lambda.Compile()();
        }

        IQueryable<TEntity> IReadableDomainContext.GetQueryableSource<TEntity>()
        {
            return _dataConnection.GetTable<TEntity>();
        }

        void IModifiableDomainContext.Add<TEntity>(TEntity entity)
        {
            _added.Add(entity);
        }

        void IModifiableDomainContext.AddRange<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _added.Add(entity);
            }
        }

        void IModifiableDomainContext.Update<TEntity>(TEntity entity)
        {
            _updated.Add(entity);
        }

        void IModifiableDomainContext.Remove<TEntity>(TEntity entity)
        {
            _deleted.Add(entity);
        }

        void IModifiableDomainContext.RemoveRange<TEntity>(IEnumerable<TEntity> entitiesToDeletePhysically)
        {
            foreach (var entity in entitiesToDeletePhysically)
            {
                _deleted.Add(entity);
            }
        }

        int IModifiableDomainContext.SaveChanges()
        {
            var count = 0;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
            {
                foreach (var addedEntity in _added)
                {
                    _dataConnection.Insert(addedEntity);
                    ++count;
                }

                foreach (var updatedEntity in _updated)
                {
                    _dataConnection.Insert(updatedEntity);
                    ++count;
                }

                foreach (var deletedEntity in _deleted)
                {
                    _dataConnection.Insert(deletedEntity);
                    ++count;
                }

                transaction.Complete();
            }
            
            _added.Clear();
            _updated.Clear();
            _deleted.Clear();

            return count;
        }

        void IDisposable.Dispose()
        {
            if (_dataConnection != null)
            {
                _pendingChangesHandlingStrategy.HandlePendingChanges(this);
                _dataConnection.Dispose();
            }
        }
    }
}