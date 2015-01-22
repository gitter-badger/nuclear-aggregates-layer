using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify
{
    public interface IModifySimplifiedModelEntityService : IOperation<ModifySimplifiedModelEntityIdentity>
    {
    }

    public interface IModifySimplifiedModelEntityService<TEntity> : IEntityOperation<TEntity>, IModifyDomainEntityService, IModifySimplifiedModelEntityService
        where TEntity : class, IEntity, IEntityKey
    {
    }
}