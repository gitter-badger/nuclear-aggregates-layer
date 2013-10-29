using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IRepositoryProxy<TEntity> where TEntity : class, IEntity
    {
        void Add(TEntity entity);
        void Update(TEntity entity, IDomainEntityAccessor<TEntity> domainEntityAccessor);
        void Delete(TEntity entity, IDomainEntityAccessor<TEntity> domainEntityAccessor);
        int Save();
    }
}