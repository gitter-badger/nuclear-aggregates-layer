using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IObjectSet<TEntity> where TEntity : class
    {
        MergeOption MergeOption { get; set; }
        EntitySet EntitySet { get; }

        TEntity ApplyCurrentValues(TEntity entity);

        void AddObject(TEntity entity);
        void DeleteObject(TEntity entity);
        
        void Attach(TEntity entity);
        void Detach(TEntity entity);

        IQueryable<TEntity> AsQueryable();
    }
}