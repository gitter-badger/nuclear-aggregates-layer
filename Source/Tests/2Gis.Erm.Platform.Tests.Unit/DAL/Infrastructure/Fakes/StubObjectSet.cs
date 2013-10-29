using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    public class StubObjectSet<TEntity> : Platform.DAL.EntityFramework.IObjectSet<TEntity> where TEntity : class
    {
        public MergeOption MergeOption { get; set; }
        public EntitySet EntitySet { get; set; }

        public TEntity ApplyCurrentValues(TEntity entity)
        {
            return entity;
        }

        public void AddObject(TEntity entity)
        {
        }

        public void DeleteObject(TEntity entity)
        {
        }

        public void Attach(TEntity entity)
        {
        }

        public void Detach(TEntity entity)
        {
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return Enumerable.Empty<TEntity>().AsQueryable();
        }
    }
}