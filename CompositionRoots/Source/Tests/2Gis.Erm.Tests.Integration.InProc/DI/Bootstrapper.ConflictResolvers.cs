using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI
{
    internal static partial class Bootstrapper
    {
        private static class MultipleImplementationResolvers
        {
            public static class EntitySpecific
            {
                public static Type ServerSidePreferable(
                    Type operationType,
                    EntitySet operationSpecificTypes,
                    IEnumerable<Type> conflictingTypes)
                {
                    return MultipleImplementationResolvers.ServerSidePreferable(conflictingTypes);
                }

                public static Type UseFirst(
                    Type operationType,
                    EntitySet operationSpecificTypes,
                    IEnumerable<Type> conflictingTypes)
                {
                    return conflictingTypes.First();
                }
            }

            public static class NonCoupled
            {
                public static Type ServerSidePreferable(Type operationType, IEnumerable<Type> conflictingTypes)
                {
                    return MultipleImplementationResolvers.ServerSidePreferable(conflictingTypes);
                }

                public static Type UseFirst(Type operationType, IEnumerable<Type> conflictingTypes)
                {
                    return conflictingTypes.First();
                }
            }

            private static Type ServerSidePreferable(IEnumerable<Type> conflictingTypes)
            {
                var types = conflictingTypes as Type[] ?? conflictingTypes.ToArray();
                return types.FirstOrDefault(conflictingType => !conflictingType.Namespace.Contains("WPF"));
            }
        }
    }
}
