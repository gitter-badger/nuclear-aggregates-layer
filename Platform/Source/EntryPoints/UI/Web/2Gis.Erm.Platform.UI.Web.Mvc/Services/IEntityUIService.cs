using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Services
{
    /// <summary>
    /// Маркерный интерфейс для сервиса, выполняющего действия в контексте агрегата системы для формирования клиенского UI
    /// </summary>
    public interface IEntityUIService : IUIService
    {
    }

    /// <summary>
    /// Маркерный интерфейс для сервиса, выполняющего действия в контексте одной конкретной агрегатной сущности системы для формирования клиенского UI
    /// </summary>
    public interface IEntityUIService<TEntity> : IEntityUIService where TEntity : class, IEntityKey
    {
    }
}