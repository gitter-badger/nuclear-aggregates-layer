using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules
{
    /// <summary>
    /// Интерфейс контейнера модулей (сборки с модулями) для клиентского WPF приложения
    /// Выполняет общее конфигурирование для всех модулей контейнера (сборки)
    /// </summary>
    public interface IModulesContainer
    {
        Guid Id { get; }
        string Description { get; }
        void Configure();
    }
}
