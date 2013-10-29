using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder
{
    [Serializable]
    public sealed class SettingsDescriptor
    {
        /// <summary>
        /// Тип реализующий контракты настроек - т.е. конкретная реализация
        /// </summary>
        public string Implementation { get; set; }
        
        /// <summary>
        /// Список реализованных интерфейсов настроек для данной конкретной реализации
        /// </summary>
        public string[] Interfaces { get; set; }
    }
}
