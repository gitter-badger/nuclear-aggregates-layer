using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public class StubGenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        public void Add(TEntity entity)
        {
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
        }

        public void Update(TEntity entity)
        {
        }

        public void Delete(TEntity entity)
        {
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
        }

        public int Save()
        {
            return 0;
        }
    }
}