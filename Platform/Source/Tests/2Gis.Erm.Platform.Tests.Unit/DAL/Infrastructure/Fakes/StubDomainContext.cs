using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        public bool IsDisposed { get; private set; }

        #region Implementation of IDisposable

        void IDisposable.Dispose()
        {
            IsDisposed = true;
        }

        public IQueryable GetQueryableSource(Type entityType)
        {
            throw new NotImplementedException();
        }

        IQueryable<TEntity> IReadDomainContext.GetQueryableSource<TEntity>()
        {
            return new TEntity[0].AsQueryable();
        }

        #endregion

        #region Implementation of IModifiableDomainContext

        public bool AnyPendingChanges
        {
            get { return !IsChangesSaved || !IsChangesAccepted; }
        }

        public bool IsChangesSaved { get; private set; }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entitiesToDeletePhysically) where TEntity : class
        {
            throw new NotImplementedException();
        }

        int IModifiableDomainContext.SaveChanges()
        {
            IsChangesSaved = true;
            return 1;
        }

        public bool IsChangesAccepted { get; private set; }

        #endregion
    }
}