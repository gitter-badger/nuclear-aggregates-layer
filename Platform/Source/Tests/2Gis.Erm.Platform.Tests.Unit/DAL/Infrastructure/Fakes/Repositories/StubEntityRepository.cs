using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public class StubEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;
        private readonly IModifiableDomainContextProvider _modifiableDomainContextProvider;

        private readonly StubDomainContext _usedModifiableDomainContext;
        private readonly StubDomainContext _usedReadDomainContext;

        private bool _isSaved;

        public StubEntityRepository(IReadDomainContextProvider readDomainContextProvider,
                                    IModifiableDomainContextProvider modifiableDomainContextProvider)
        {
            _readDomainContextProvider = readDomainContextProvider;
            _modifiableDomainContextProvider = modifiableDomainContextProvider;

            _usedModifiableDomainContext = ModifiableDomainContextProvider.Get<TEntity>() as StubDomainContext;
            if (_usedModifiableDomainContext == null)
            {
                throw new InvalidOperationException(
                    "Unsupported type of domain context was resolved through context provider. Domain context must be assignable to type: " +
                    typeof(StubDomainContext).Name);
            }

            _usedReadDomainContext = ReadDomainContextProvider.Get() as StubDomainContext;
            if (UsedReadDomainContext == null)
            {
                throw new InvalidOperationException(
                    "Unsupported type of domain context was resolved through context provider. Domain context must be assignable to type: " +
                    typeof(StubDomainContext).Name);
            }
        }

        public bool IsSaved
        {
            get
            {
                return _isSaved;
            }
        }

        public StubDomainContext UsedModifiableDomainContext
        {
            get
            {
                return _usedModifiableDomainContext;
            }
        }

        public IModifiableDomainContextProvider ModifiableDomainContextProvider
        {
            get
            {
                return _modifiableDomainContextProvider;
            }
        }

        public IReadDomainContextProvider ReadDomainContextProvider
        {
            get
            {
                return _readDomainContextProvider;
            }
        }

        public StubDomainContext UsedReadDomainContext
        {
            get
            {
                return _usedReadDomainContext;
            }
        }

        #region Implementation of IRepository<TEntity>

        public void Add(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        public int Save()
        {
            _isSaved = true;

            return ((IModifiableDomainContext)_usedModifiableDomainContext).SaveChanges();
        }

        public void Update(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Attach(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public IQueryable<TEntity> Find(IFindSpecification<TEntity> findSpecification)
        {
            throw new NotSupportedException();
        }

        public IQueryable<TOutput> Find<TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification, IFindSpecification<TEntity> findSpecification)
        {
            throw new NotSupportedException();
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
