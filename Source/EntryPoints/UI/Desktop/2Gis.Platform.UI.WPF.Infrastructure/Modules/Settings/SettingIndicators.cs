using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings
{
    /// <summary>
    /// Контейнер индикаторов - маркерных интерфейсов, связанных с инфраструктурой настроек
    /// Назначение - использование во всяких выражениях проверок относится ли что-то к операциям  и т.п. - вида typeof(ISetting).IsAssignableFrom(checkingType)
    /// </summary>
    public static class SettingIndicators
    {
        /// <summary>
        /// Контейнер модулей
        /// </summary>
        public static readonly Type Settings = typeof(ISettings);
    }
}
