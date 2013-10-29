using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// Маркерный интерфейс для DTO специфичной для конкретной операции
    /// </summary>
    public interface IOperationSpecificEntityDto
    {
    }

    /// <summary>
    /// Маркерный интерфейс для DTO специфичной для конкретного типа сущности для конкретной операции
    /// </summary>
    public interface IOperationSpecificEntityDto<TEntity> : IOperationSpecificEntityDto
        where TEntity : IEntityKey
    {
    }
}
