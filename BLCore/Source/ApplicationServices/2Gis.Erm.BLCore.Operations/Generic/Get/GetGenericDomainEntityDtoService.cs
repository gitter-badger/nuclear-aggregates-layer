using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetGenericDomainEntityDtoService<TEntity> : GetDomainEntityDtoServiceBase<TEntity> 
        where TEntity : class, IEntityKey, IEntity
    {
        public GetGenericDomainEntityDtoService(IUserContext userContext) : base(userContext)
        {
        }

        protected override IDomainEntityDto<TEntity> GetDto(long entityId)
        {
            throw new System.NotSupportedException();
        }

        protected override IDomainEntityDto<TEntity> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            throw new System.NotSupportedException();
        }
    }
}