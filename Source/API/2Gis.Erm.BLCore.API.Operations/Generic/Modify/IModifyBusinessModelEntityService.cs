using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify
{
    public interface IModifyBusinessModelEntityService : IOperation<ModifyBusinessModelEntityIdentity>
    {
    }

    public interface IModifyBusinessModelEntityService<TEntity> : IEntityOperation<TEntity>, IModifyDomainEntityService, IModifyBusinessModelEntityService
        where TEntity : class, IEntity, IEntityKey
    {
    }
}