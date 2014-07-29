using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Update
{
    public interface IUpdateOperationService : IOperation<UpdateIdentity>
    {
        void Update(IDomainEntityDto entityDto);
    }

    public interface IUpdateOperationService<TEntity> : IEntityOperation<TEntity>, IUpdateOperationService
        where TEntity : class, IEntity, IEntityKey
    {
    }
}
