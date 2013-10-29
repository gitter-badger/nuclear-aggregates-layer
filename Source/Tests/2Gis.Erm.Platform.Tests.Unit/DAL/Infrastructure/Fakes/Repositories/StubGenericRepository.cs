using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public class StubGenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        public void Add(TEntity entity)
        {
        }

        public void Update(TEntity entity)
        {
        }

        public void Delete(TEntity entity)
        {
        }

        public int Save()
        {
            return 0;
        }
    }
}