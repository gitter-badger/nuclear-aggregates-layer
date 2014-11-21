﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Services.Operations.OrderProlongationRequestOperationServiceTests
{
    class FakeSecureFinder : ISecureFinder
    {
        public FakeSecureFinder()
        {
            Storage = new List<object>();
        }

        public List<object> Storage { get; private set; }

        public IQueryable FindAll(Type entityType)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> FindAll<TEntity>() where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            return Storage.OfType<TEntity>().Where(findSpecification.Predicate.Compile()).AsQueryable();
        }

        public IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification, IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
        {
            throw new NotImplementedException();
        }

        public TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity, IEntityKey
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity, IEntityKey
        {
            return Storage.OfType<TEntity>().Where(findSpecification.Predicate.Compile()).ToArray();
        }
    }
}
