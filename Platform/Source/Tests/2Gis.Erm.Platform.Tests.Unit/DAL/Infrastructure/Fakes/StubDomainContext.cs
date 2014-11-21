using System;
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

        int IModifiableDomainContext.SaveChanges(SaveOptions options)
        {
            IsChangesSaved = true;
            return 1;
        }

        public bool IsChangesAccepted { get; private set; }

        void IModifiableDomainContext.AcceptAllChanges()
        {
            IsChangesAccepted = true;
        }

        #endregion
    }
}