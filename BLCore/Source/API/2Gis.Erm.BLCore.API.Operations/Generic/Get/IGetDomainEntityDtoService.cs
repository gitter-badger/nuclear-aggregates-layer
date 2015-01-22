﻿using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Get
{
    public interface IGetDomainEntityDtoService : IOperation<GetDomainEntityDtoIdentity>
    {
        IDomainEntityDto GetDomainEntityDto(long entityId, bool readOnly, long? parentEntityId, EntityName parentEntityName, string extendedInfo);
    }

    public interface IGetDomainEntityDtoService<TEntity> : IEntityOperation<TEntity>, IGetDomainEntityDtoService
        where TEntity : class, IEntityKey
    {
    }
}