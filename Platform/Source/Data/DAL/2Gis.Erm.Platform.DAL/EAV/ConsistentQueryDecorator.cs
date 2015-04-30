using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    // ReSharper disable once RedundantExtendsListEntry
    public class ConsistentQueryDecorator : IQuery, ISecureQuery
    {
        private readonly IQuery _query;

        public ConsistentQueryDecorator(IQuery query)
        {
            _query = query;
        }

        public IQueryable For(Type entityType)
        {
            return _query.For(entityType).ValidateQueryCorrectness();
        }

        public IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity
        {
            return _query.For<TEntity>().ValidateQueryCorrectness();
        }
    }
}