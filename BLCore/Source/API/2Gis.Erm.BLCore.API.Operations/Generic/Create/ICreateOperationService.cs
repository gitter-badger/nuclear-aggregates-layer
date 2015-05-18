using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Create
{
    public interface ICreateOperationService : IOperation<CreateIdentity>
    {
        long Create(IDomainEntityDto entityDto);
    }

    public interface ICreateOperationService<TEntity> : IEntityOperation<TEntity>, ICreateOperationService
        where TEntity : class, IEntity, IEntityKey
    {
    }
}
