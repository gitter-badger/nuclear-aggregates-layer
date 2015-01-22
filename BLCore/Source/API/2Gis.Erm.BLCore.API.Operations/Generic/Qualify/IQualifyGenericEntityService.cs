using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Qualify
{
    public interface IQualifyGenericEntityService<TEntity> : IEntityOperation<TEntity>, IQualifyEntityService 
        where TEntity : class, IEntityKey
    {
    }
}