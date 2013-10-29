using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    /// <summary>
    /// Маркерный интерфейс для агрегирующих (типизированных, специфических) репозиториев частей контракта операций агрегата
    /// Т.е. расширять этот интерфейс должны интерфейсы определяющие часть контракта операций агрегата для какой-то конкретной сущности - составной части агрегата
    /// </summary>
    public interface IAggregatePartRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}