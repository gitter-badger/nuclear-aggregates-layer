using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    public class Finder : IFinder
    {
        private readonly IReadDomainContextProvider _readDomainContextProvider;

        public Finder(IReadDomainContextProvider readDomainContextProvider)
        {
            if (readDomainContextProvider == null)
            {
                throw new ArgumentNullException("readDomainContextProvider");
            }

            _readDomainContextProvider = readDomainContextProvider;
        }

        IQueryable IFinderBase.FindAll(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            return _readDomainContextProvider.Get().GetQueryableSource(entityType);
        }

        IQueryable<TEntity> IFinderBase.FindAll<TEntity>()
        {
            return _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
        }

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) 
            where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate);
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification,
                                                          IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (selectSpecification == null)
            {
                throw new ArgumentNullException("selectSpecification");
            }
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate).Select(selectSpecification.Selector);
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) 
            where TEntity : class, IEntity
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(expression);
        }

        public TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            throw new NotSupportedException("ConsistentFinderDecorator should be used");
        }

        public IEnumerable<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            throw new NotSupportedException("ConsistentFinderDecorator should be used");
        }
    }
}
