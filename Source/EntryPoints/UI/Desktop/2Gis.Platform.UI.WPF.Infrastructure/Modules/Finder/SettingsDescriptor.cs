using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Finder
{
    [Serializable]
    public sealed class SettingsContainerDescriptor
    {
        /// <summary>
        /// Тип реализующий контракты настроек - т.е. конкретная реализация
        /// </summary>
        public string Implementation { get; set; }
    }
}
