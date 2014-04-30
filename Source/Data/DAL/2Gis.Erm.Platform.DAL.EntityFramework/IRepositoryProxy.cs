using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IRepositoryProxy<TEntity> where TEntity : class, IEntity
    {
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor);
        void Delete(TEntity entity, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor);
        void DeleteRange(IEnumerable<TEntity> entities, IDomainEntityEntryAccessor<TEntity> domainEntityEntryAccessor);
        int Save();
    }
}