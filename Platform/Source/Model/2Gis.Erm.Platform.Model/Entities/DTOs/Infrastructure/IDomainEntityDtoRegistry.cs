using System;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure
{
    public interface IDomainEntityDtoRegistry
    {
        bool TryGetDomainEntityDto(EntityName entityName, out Type domainEntityDtoType);
    }
}