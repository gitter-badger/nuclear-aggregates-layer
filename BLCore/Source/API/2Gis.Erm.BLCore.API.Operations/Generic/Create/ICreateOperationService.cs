using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

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
