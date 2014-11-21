using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public enum EntityPlacementState
    {
        CachedInContext = 1,
        AttachedToContext = 2
    }

    public interface IDomainEntityEntryAccessor<in TEntity> where TEntity : class, IEntity
    {
        IDbEntityEntry GetDomainEntityEntry(TEntity entity, out EntityPlacementState entityPlacementState);
    }
}