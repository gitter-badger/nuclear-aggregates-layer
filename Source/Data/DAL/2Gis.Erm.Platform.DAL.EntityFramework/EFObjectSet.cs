using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EFObjectSet<TEntity> : IObjectSet<TEntity> where TEntity : class
    {
        private readonly ObjectSet<TEntity> _objectSet;

        public EFObjectSet(ObjectSet<TEntity> objectSet)
        {
            _objectSet = objectSet;
        }

        public MergeOption MergeOption
        {
            get { return _objectSet.MergeOption; } 
            set { _objectSet.MergeOption = value; }
        }

        public EntitySet EntitySet
        {
            get { return _objectSet.EntitySet; }
        }

        public TEntity ApplyCurrentValues(TEntity entity)
        {
            return _objectSet.ApplyCurrentValues(entity);
        }

        public void AddObject(TEntity entity)
        {
            _objectSet.AddObject(entity);
        }

        public void DeleteObject(TEntity entity)
        {
            _objectSet.DeleteObject(entity);
        }

        public void Attach(TEntity entity)
        {
            _objectSet.Attach(entity);
        }

        public void Detach(TEntity entity)
        {
            _objectSet.Detach(entity);
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return _objectSet;
        }
    }
}