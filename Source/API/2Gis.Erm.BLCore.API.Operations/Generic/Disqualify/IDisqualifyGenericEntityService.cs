using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify
{
    public interface IDisqualifyGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDisqualifyEntityService 
        where TEntity : class, IEntityKey
    {
    }
}