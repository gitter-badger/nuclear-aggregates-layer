using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IDomainEntityAccessor<TEntity> where TEntity : class, IEntity
    {
        TEntity GetDomainEntity(TEntity entity);
    }
}