using System;

namespace DoubleGis.Erm.Platform.Model
{
    public static partial class ModelIndicators
    {
        public static class Boundaries
        {
            public static readonly Type BoundedContext = typeof(IBoundedContext);

            /// <summary>
            /// Группы индикаторов
            /// </summary>
            public static class Group
            {
                /// <summary>
                /// Все маркерные интерфейсы для инфраструктуры bounded countexts
                /// </summary>
                public static readonly Type[] All =
                    {
                        BoundedContext
                    };
            }
        }

        public static bool IsBoundedContext(this Type checkingType)
        {
            return Boundaries.BoundedContext.IsAssignableFrom(checkingType);
        }
    }
}
