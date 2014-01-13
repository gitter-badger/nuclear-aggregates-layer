using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    /// <summary>
    /// Маркерный интерфейс DTO, используемого в операции List, для конкретного типа сущности ERM
    /// </summary>
    public interface IListItemEntityDto<TEntity> : IOperationSpecificEntityDto<TEntity>
        where TEntity : IEntityKey
    {
    }
}