using System;
using System.Collections.Generic;

using NuClear.Storage.Core;
using NuClear.Storage.Writings;

namespace NuClear.Storage.LinqToDB
{
    public class LinqToDBRepository<TDomainEntity> : IRepository<TDomainEntity> where TDomainEntity : class
    {
        private readonly IModifiableDomainContext _domainContext;

        public LinqToDBRepository(IModifiableDomainContextProvider domainContextProvider)
        {
            _domainContext = domainContextProvider.Get<TDomainEntity>() as LinqToDBDomainContext;
            if (_domainContext == null)
            {
                throw new ArgumentException("IModifiableDomainContextProvider implementation must provide instance of LinqToDBDomainContext");
            }
        }

        public void Add(TDomainEntity entity)
        {
            _domainContext.Add(entity);
        }

        public void AddRange(IEnumerable<TDomainEntity> entities)
        {
            _domainContext.AddRange(entities);
        }

        public void Update(TDomainEntity entity)
        {
            _domainContext.Update(entity);
        }

        public void Delete(TDomainEntity entity)
        {
            _domainContext.Remove(entity);
        }

        public void DeleteRange(IEnumerable<TDomainEntity> entities)
        {
            _domainContext.RemoveRange(entities);
        }

        public int Save()
        {
            return _domainContext.SaveChanges();
        }
    }
}