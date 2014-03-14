using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    public static class BusinessModelsIndicators
    {
        public static readonly Type Adapted = typeof(IAdapted);

        public static class Group
        {
            /// <summary>
            /// Все маркерные интерфейсы для раличных типов обслуживающих работу с настройками
            /// </summary>
            public static readonly Type[] All =
                    {
                        Adapted
                    };
        }

        public static bool IsAdaptedType(this Type checkingType)
        {
            return Adapted.IsAssignableFrom(checkingType);
        }
    }
}
