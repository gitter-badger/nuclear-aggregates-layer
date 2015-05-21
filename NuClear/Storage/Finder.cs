using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
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

        public IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) 
            where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate);
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification,
                                                          FindSpecification<TEntity> findSpecification)
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

        public TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate).SingleOrDefault();
        }

        public TOutput FindOne<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification, FindSpecification<TEntity> findSpecification) 
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
            return queryableSource.Where(findSpecification.Predicate).Select(selectSpecification.Selector).SingleOrDefault();
        }

        public IReadOnlyCollection<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(findSpecification.Predicate).ToArray();
        }

        public IReadOnlyCollection<TOutput> FindMany<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification, FindSpecification<TEntity> findSpecification) 
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
            return queryableSource.Where(findSpecification.Predicate).Select(selectSpecification.Selector).ToArray();
        }

        public bool FindAny<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            if (findSpecification == null)
            {
                throw new ArgumentNullException("findSpecification");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Any(findSpecification.Predicate);
        }
    }
}
