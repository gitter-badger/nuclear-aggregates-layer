using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure
{
    public static class DomainEntityDtoUtils
    {
        public static bool IsNew<TEntity>(this IDomainEntityDto<TEntity> domainEntityDto) 
            where TEntity : class, IEntity, IEntityKey
        {
            IDomainEntityDto dto = domainEntityDto;
            return dto.IsNew();
        }

        public static bool IsNew(this IDomainEntityDto domainEntityDto)
        {
            return domainEntityDto.Id == 0;
        }
    }
}
