using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Update;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update
{
    public abstract class UpdateOperationServiceBase<TEntity, TDomainEntityDto> : IUpdateOperationService<TEntity> 
        where TEntity : class, IEntity, IEntityKey, new()
        where TDomainEntityDto : class, IDomainEntityDto<TEntity>, new()
    {
        private readonly IBusinessModelEntityObtainer<TEntity> _entityObtainer;

        protected UpdateOperationServiceBase(IBusinessModelEntityObtainer<TEntity> entityObtainer)
        {
            _entityObtainer = entityObtainer;
        }

        public void Update(IDomainEntityDto entityDto)
        {
            var entity = _entityObtainer.ObtainBusinessModelEntity(entityDto);
            Update(entity, (TDomainEntityDto)entityDto);
        }

        protected abstract void Update(TEntity entity, TDomainEntityDto entityDto);
    }
}
