using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Disqualify
{
    public interface IDisqualifyGenericEntityService<TEntity> : IEntityOperation<TEntity>, IDisqualifyEntityService 
        where TEntity : class, IEntityKey
    {
    }
}