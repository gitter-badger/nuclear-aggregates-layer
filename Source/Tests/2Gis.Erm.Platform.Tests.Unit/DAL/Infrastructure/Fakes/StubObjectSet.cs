using System.Collections.Generic;
using System.Data.Entity;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubObjectSet<TEntity> : DbSet<TEntity> where TEntity : class
    {
        public override TEntity Add(TEntity entity)
        {
            return entity;
        }

        public override IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            return entities;
        }

        public override TEntity Remove(TEntity entity)
        {
            return entity;
        }

        public override IEnumerable<TEntity> RemoveRange(IEnumerable<TEntity> entities)
        {
            return entities;
        }

        public override TEntity Attach(TEntity entity)
        {
            return entity;
        }
    }
}