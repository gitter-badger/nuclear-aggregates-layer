using System;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    public static class SettingsIndicators
    {
        public static readonly Type Settings = typeof(ISettings);
        public static readonly Type SettingsAspect = typeof(ISettingsAspect);
        public static readonly Type SettingsContainer = typeof(ISettingsContainer);

        public static bool IsSettings(this Type checkingType)
        {
            return Settings.IsAssignableFrom(checkingType);
        }

        public static bool IsSettingsAspect(this Type checkingType)
        {
            return SettingsAspect.IsAssignableFrom(checkingType);
        }

        public static bool IsSettingsContainer(this Type checkingType)
        {
            return SettingsContainer.IsAssignableFrom(checkingType);
        }

        public static class Group
        {
            /// <summary>
            /// Все маркерные интерфейсы для раличных типов обслуживающих работу с настройками
            /// </summary>
            public static readonly Type[] All =
                    {
                        Settings,
                        SettingsAspect,
                        SettingsContainer
                    };
        }
    }
}