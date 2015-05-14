using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    class FakeSecureFinder : ISecureFinder
    {
        public FakeSecureFinder()
        {
            Storage = new List<object>();
        }

        public List<object> Storage { get; private set; }

        public IQueryable For(Type entityType)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return Storage.OfType<TEntity>().Where(findSpecification.Predicate.Compile()).AsQueryable();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(SelectSpecification<TEntity, TOutput> selectSpecification, FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return Storage.OfType<TEntity>().Where(findSpecification.Predicate.Compile()).ToArray();
        }
    }
}
