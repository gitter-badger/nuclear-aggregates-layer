using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules
{
    /// <summary>
    /// Интерфейс модуля для клиентского WPF приложения 
    /// </summary>
    public interface IModule
    {
        Guid Id { get; }
        string Description { get; }
        void Configure();
    }
}
