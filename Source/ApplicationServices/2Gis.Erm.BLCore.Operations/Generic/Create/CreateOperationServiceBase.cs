using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Create
{
    public abstract class CreateOperationServiceBase<TEntity, TDomainEntityDto> : ICreateOperationService<TEntity> 
        where TEntity : class, IEntity, IEntityKey, new()
        where TDomainEntityDto : class, IDomainEntityDto<TEntity>, new()
    {
        public long Create(IDomainEntityDto entityDto)
        {
            var entity = new TEntity();
            var typedEntityDto = (TDomainEntityDto)entityDto;
            MapProperties(entity, typedEntityDto);
            return Create(entity, typedEntityDto);
        }

        protected abstract long Create(TEntity entity, TDomainEntityDto entityDto);

        protected virtual void MapProperties(TEntity entity, TDomainEntityDto entityDto)
        {
        }
    }
}
