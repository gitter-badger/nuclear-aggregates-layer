using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules
{
    /// <summary>
    /// Контейнер индикаторов - маркерных интерфейсов, связанных с инфраструктурой модулей
    /// Назначение - использование во всяких выражениях проверок относится ли что-то к операциям  и т.п. - вида typeof(IModule).IsAssignableFrom(checkingType)
    /// </summary>
    public static class ModuleIndicators
    {
        /// <summary>
        /// Контейнер модулей
        /// </summary>
        public static Type Container
        {
            get { return typeof(IModulesContainer); }
        }

        /// <summary>
        /// Модуль
        /// </summary>
        public static Type Module
        {
            get { return typeof(IModule); }
        }
        
        /// <summary>
        /// Модуль, кроме прочего выполняющий какие-то background задачи
        /// </summary> 
        public static Type StandaloneWorkerModule
        {
            get { return typeof(IStandaloneWorkerModule); }
        }

        public static class Group
        {
            /// <summary>
            /// Все маркерные интерфейсы инфраструктуры модулей
            /// </summary>
            public static Type[] All
            {
                get
                {
                    return new[]
                        {
                            Container,
                            Module,
                            StandaloneWorkerModule
                        };
                }
            }
        }
    }
}
