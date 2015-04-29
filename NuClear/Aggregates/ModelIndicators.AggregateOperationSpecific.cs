using System;

namespace NuClear.Aggregates
{
    public static partial class Indicators
    {
        public static class Operations
        {
            public static readonly Type OperationSpecific = typeof(IAggregateSpecificOperation<,>);
            public static readonly Type OperationSpecificUnknownAggregate = typeof(IUnknownAggregateSpecificOperation<>);

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
                        OperationSpecific,
                        OperationSpecificUnknownAggregate
                    };

                /// <summary>
                /// Маркерные интерфейсы для слоя агрегатов - updatable части
                /// </summary>
                public static readonly Type[] Repositories =
                    {
                        OperationSpecific,
                        OperationSpecificUnknownAggregate
                    };
            }
        }
    }
}