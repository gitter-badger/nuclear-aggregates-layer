using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories
{
    public class EFGenericRepositoryWrapper<TEntity> : IRepository<TEntity>, IDomainEntityAccessor<TEntity> where TEntity : class, IEntity
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


        public int Save()
        {
            return _genericRepository.Save();
        }

        public TEntity GetDomainEntity(TEntity entity)
        {
            return entity;
        }
    }
}