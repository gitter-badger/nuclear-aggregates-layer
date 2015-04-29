using System;

using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// Маркерный интерфейс для агрегирующих (типизированных, специфических) репозиториев частей контракта операций агрегата
    /// Т.е. расширять этот интерфейс должны интерфейсы определяющие часть контракта операций агрегата для какой-то конкретной сущности - составной части агрегата
    /// </summary>
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IAggregatePartRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}