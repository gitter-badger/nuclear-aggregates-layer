using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

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
