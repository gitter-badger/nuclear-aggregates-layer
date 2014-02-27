using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class MockFinder : IFinder
    {
        readonly IList<object> _list;

        public MockFinder()
        {
            _list = new List<object>();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            _list.Add(entity);
        }

        public IQueryable FindAll(Type entityType)
        {
            var items = _list.Where(o => o.GetType() == entityType);

            return items.Cast<IEntityKey>().AsQueryable<IEntityKey>();
        }

        public IQueryable<TEntity> FindAll<TEntity>() where TEntity : class, IEntity
        {
            var items = _list.Where(o => (o is TEntity));

            return items.Cast<TEntity>().AsQueryable<TEntity>();
        }

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotSupportedException();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification, IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotSupportedException();
        }

        public TOutput Find<TEntity, TQuery, TOutput>(ISelectSpecification<TQuery, TOutput> selectSpecification, IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity
            where TQuery : IQueryable<TEntity>
        {
            throw new NotSupportedException();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            var f = expression.Compile();
            return FindAll<TEntity>().Where(e => f(e)).AsQueryable();
        }
    }
}