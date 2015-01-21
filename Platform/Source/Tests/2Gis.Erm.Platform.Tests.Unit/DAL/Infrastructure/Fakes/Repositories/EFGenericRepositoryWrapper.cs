using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public class EFGenericRepositoryWrapper<TEntity> : IRepository<TEntity>, IDomainEntityEntryAccessor<TEntity> where TEntity : class, IEntity
    {
        readonly EFGenericRepository<TEntity> _genericRepository;

        public EFGenericRepositoryWrapper(EFGenericRepository<TEntity> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public void Add(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)_genericRepository;
            repositoryProxy.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)_genericRepository;
            repositoryProxy.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)_genericRepository;
            repositoryProxy.Update(entity, this);
        }

        public void Delete(TEntity entity)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)_genericRepository;
            repositoryProxy.Delete(entity, this);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            var repositoryProxy = (IRepositoryProxy<TEntity>)_genericRepository;
            repositoryProxy.DeleteRange(entities, this);
        }

        public int Save()
        {
            return _genericRepository.Save();
        }

        public TEntity GetDomainEntity(TEntity entity)
        {
            return entity;
        }

        public IDbEntityEntry GetDomainEntityEntry(TEntity entity, out EntityPlacementState entityPlacementState)
        {
            entityPlacementState = EntityPlacementState.AttachedToContext;
            return new EFEntityEntry(null);
        }
    }
}