using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Маркерный интерфейс persistence-сервисов - эти сервисы реализуют уровень persistance ignorance, реализуя паттерн Repository,
    /// либо другой специфический контракт, отделяющий BL от деталей хранения
    /// </summary>
    public interface IPersistenceService
    {
    }

    /// <summary>
    /// Маркерный интерфейс для persistence-сервисов, представляющих собой абстракцию над хранилищем данных
    /// для использования в агрегирующих репозиториях
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности, с которой работает сервис.</typeparam>
    public interface IPersistenceService<TEntity> : IPersistenceService where TEntity : class, IEntity
    {
    }

    /// <summary>
    /// Маркерный интерфейс для упрощенных persistence-сервисов, работающих вне агрегирующих репозиториев
    /// и не привязанных к какой-либо бизнес-сущности
    /// </summary>
    public interface ISimplifiedPersistenceService : IPersistenceService
    {
    }
}