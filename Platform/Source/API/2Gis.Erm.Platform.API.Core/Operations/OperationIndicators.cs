using System;
using System.Linq;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// Контейнер индикаторов - маркерных интерфейсов, связанных с инфраструктурой операций и метаданных
    /// Назначение - использование во всяких выражениях проверок относится ли что-то к операциям  и т.п. - вида typeof(IOperation).IsAssignableFrom(checkingType)
    /// </summary>
    public static class OperationIndicators
    {
        /// <summary>
        /// Индикатор маркерного интерфейса операции
        /// </summary>
        public static readonly Type Operation = typeof(IOperation);

        /// <summary>
        /// Индикатор маркерного интерфейса операции - generic версии связывающей операцию с operation identity
        /// </summary>
        public static readonly Type IdentifiedOperation = typeof(IOperation<>);

        /// <summary>
        /// Индикатор маркерного интерфейса сущностно специфичной операции
        /// </summary>
        public static readonly Type EntitySpecificOperation = typeof(IEntityOperation);

        /// <summary>
        /// Индикатор маркерного интерфейса сущностно специфичной операции - generic версия - операция зависит от одной сущности
        /// </summary>
        public static readonly Type Entity1SpecificOperation = typeof(IEntityOperation<>);

        /// <summary>
        /// Индикатор маркерного интерфейса сущностно специфичной операции - generic версия - операция зависит от двух сущностей
        /// </summary>
        public static readonly Type Entity2SpecificOperation = typeof(IEntityOperation<,>);

        /// <summary>
        /// Индикатор маркерного интерфейса operation identity
        /// </summary>
        public static readonly Type OperationIdentity = typeof(IOperationIdentity);

        /// <summary>
        /// Индикатор маркерного интерфейса operation metadata detail - деталей метаданных специфиных для конкретной операции (возможно её реализации для конкретных типов сущностей)
        /// </summary>
        public static readonly Type OperationMetadataDetailIndicator = typeof(IOperationMetadata);

        /// <summary>
        /// Группы индикаторов
        /// </summary>
        public static class Group
        {
            /// <summary>
            /// Все маркерные интерфейсы инфраструктуры операций
            /// </summary>
            public static readonly Type[] All =
            {
                Operation, 
                IdentifiedOperation, 
                EntitySpecificOperation,
                Entity1SpecificOperation, 
                Entity2SpecificOperation
            };

            /// <summary>
            /// Все маркерные интерфейсы инфраструктуры операций, НЕ зависящие от типа сущности(ей)
            /// </summary>
            public static readonly Type[] NotEntitySpecific = All.Where(t => !EntitySpecificOperation.IsAssignableFrom(t)).ToArray();

            /// <summary>
            /// Все маркерные интерфейсы инфраструктуры операций, ЗАВИСЯЩИЕ от типа сущности(ей)
            /// </summary>
            public static readonly Type[] EntitySpecific = All.Where(t => EntitySpecificOperation.IsAssignableFrom(t)).ToArray();

            /// <summary>
            /// Все маркерные интерфейсы инфраструктуры операций, ЗАВИСЯЩИЕ от типа сущности(ей) - при этом ТОЛЬКО generic интерфейсы
            /// </summary>
            public static readonly Type[] EntitySpecificGeneric = All.Where(t => t != EntitySpecificOperation && EntitySpecificOperation.IsAssignableFrom(t)).ToArray();
        }
    }
}
