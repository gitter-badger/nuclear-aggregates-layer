using System.Collections.Generic;

namespace Nuclear.Settings.API
{
    /// <summary>
    /// Базовый класс для контейнеров настроек, с возможностью подключать аспекты настроек
    /// 
    /// </summary>
    public abstract class SettingsContainerBase : ISettingsContainer
    {
        protected readonly ICollection<ISettingsAspect> Aspects = new List<ISettingsAspect>();

        public IEnumerable<ISettingsAspect> SettingsAspects
        {
            get { return Aspects; }
        }
    }
}