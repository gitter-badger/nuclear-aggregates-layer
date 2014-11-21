using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify
{
    public interface IQualifyGenericEntityService<TEntity> : IEntityOperation<TEntity>, IQualifyEntityService 
        where TEntity : class, IEntityKey
    {
    }
}