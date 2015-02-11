using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeActivityStatus
{
    public interface IChangeGenericEntityActivityStatusService<TEntity> : IEntityOperation<TEntity>, IChangeActvityStatusEntityService
        where TEntity : class, IEntityKey
    {
    }    
}
