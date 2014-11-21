using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

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