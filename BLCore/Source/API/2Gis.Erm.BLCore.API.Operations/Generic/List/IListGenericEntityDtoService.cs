using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IListGenericEntityDtoService<TEntity, TEntityListDto> : IEntityOperation<TEntity>, IListEntityService
        where TEntity : class, IEntityKey
        where TEntityListDto : IOperationSpecificEntityDto
    {
    }
}