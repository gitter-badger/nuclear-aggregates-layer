using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// Маркерный интерфейс для Read model с привязкой к агрегатам. Read model - это способ выполнять специфические для агрегата выборки. 
    /// С учетом ограничений domain model (т.е. безопасность, метаданные и т.п.), фактически фасад для спецификаций + ограничения domain model
    /// </summary>
    public interface IAggregateReadModel
    {
    }

    /// <summary>
    /// Маркерный интерфейс для Read model с привязкой к конкретному aggregate root. Read model - это способ выполнять специфические для агрегата выборки. 
    /// С учетом ограничений domain model (т.е. безопасность, метаданные и т.п.), фактически фасад для спецификаций + ограничения domain model
    /// </summary>
    public interface IAggregateReadModel<TAggregateRoot> : IAggregateReadModel
        where TAggregateRoot : class, IEntity
    {
    }
}