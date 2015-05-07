using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFSecureGenericRepository<TEntity> : ISecureRepository<TEntity> 
        where TEntity : class, IEntity, IEntityKey, ICuratedEntity
    {
        private readonly IRepository<TEntity> _repository;
        private readonly SecurityHelper _securityHelper;

        public EFSecureGenericRepository(IUserContext userContext,
                                         IRepository<TEntity> repository,
                                         ISecurityServiceEntityAccessInternal entityAccessService)
        {
            _repository = repository;
            _securityHelper = new SecurityHelper(userContext, entityAccessService);
        }

        public void Add(TEntity entity)
        {
            _securityHelper.CheckRequest(EntityAccessTypes.Create, entity);
            _repository.Add(entity);
        }

        public void Update(TEntity entity)
        {
            _securityHelper.CheckRequest(EntityAccessTypes.Update, entity);
            _repository.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _securityHelper.CheckRequest(EntityAccessTypes.Delete, entity);
            _repository.Delete(entity);
        }

        public int Save()
        {
            return _repository.Save();
        }
    }
}
