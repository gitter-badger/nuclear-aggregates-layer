using System;

using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.Model
{
    public static partial class ModelIndicators
    {
        public static class Simplified
        {
            public static readonly Type SimplifiedModelConsumer = typeof(ISimplifiedModelConsumer);
            public static readonly Type SimplifiedModelConsumerReadModel = typeof(ISimplifiedModelConsumerReadModel);

            /// <summary>
            /// Группы индикаторов
            /// </summary>
            public static class Group
            {
                /// <summary>
                /// Все маркерные интерфейсы для слоя агрегатов
                /// </summary>
                public static readonly Type[] All =
                    {
                        SimplifiedModelConsumer,
                        SimplifiedModelConsumerReadModel
                    };

                /// <summary>
                /// Маркерные интерфейсы для слоя агрегатов - readonly части
                /// </summary>
                public static readonly Type[] ReadOnly =
                    {
                        SimplifiedModelConsumerReadModel,
                    };

                /// <summary>
                /// Все маркерные интерфейсы для слоя агрегатов - simplified model
                /// </summary>
                public static readonly Type[] SimplifiedModel =
                    {
                        SimplifiedModelConsumer
                    };
            }
        }

        public static bool IsSimplifiedModelConsumerReadModel(this Type checkingType)
        {
            return Simplified.SimplifiedModelConsumerReadModel.IsAssignableFrom(checkingType);
        }

        public static bool IsSimplifiedModelConsumer(this Type checkingType)
        {
            return Simplified.SimplifiedModelConsumer.IsAssignableFrom(checkingType);
        }
    }
}
