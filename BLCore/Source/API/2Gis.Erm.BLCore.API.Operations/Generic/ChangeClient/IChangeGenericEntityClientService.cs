using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient
{
    public interface IChangeGenericEntityClientService<TEntity> : IEntityOperation<TEntity>, IChangeEntityClientService
        where TEntity : class, IEntityKey
    {
    }
}