using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeClient
{
    public interface IChangeGenericEntityClientService<TEntity> : IEntityOperation<TEntity>, IChangeEntityClientService
        where TEntity : class, IEntityKey
    {
    }
}