using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

        public TOutput Find<TEntity, TQuery, TOutput>(ISelectSpecification<TQuery, TOutput> selectSpecifications, 
            IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
            where TQuery : IQueryable<TEntity>
        {
            // TODO {d.ivanov, 29.07.2013}: Пока не знаю как реализовать
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) 
            where TEntity : class, IEntity
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var queryableSource = _readDomainContextProvider.Get().GetQueryableSource<TEntity>();
            return queryableSource.Where(new FindSpecification<TEntity>(expression).Predicate);
        }
    }
}
