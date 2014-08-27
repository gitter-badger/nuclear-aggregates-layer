using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public interface IBatchDeletePersistenceService : ISimplifiedPersistenceService
    {
        void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity, IEntityKey;
    }
}