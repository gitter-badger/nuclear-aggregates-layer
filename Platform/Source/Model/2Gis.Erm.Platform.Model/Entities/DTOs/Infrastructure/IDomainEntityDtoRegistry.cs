using System;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs.Infrastructure
{
    public interface IDomainEntityDtoRegistry
    {
        bool TryGetDomainEntityDto(IEntityType entityName, out Type domainEntityDtoType);
    }
}