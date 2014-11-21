using System;
using System.Linq;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static class TypeExtensions
    {
        public static bool CanMapTo(this Type tFrom, Type tTo)
        {
            return tFrom.IsGenericType && tFrom.IsGenericTypeDefinition
                       ? tTo.IsAssignableToGenericType(tFrom)
                       : tFrom.IsAssignableFrom(tTo);
        }

        /// <summary>
        /// Determines whether the <paramref name="genericType"/> is assignable from
        /// <paramref name="givenType"/> taking into account generic definitions
        /// </summary>
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null || genericType == null)
            {
                return false;
            }

            return givenType == genericType
              || givenType.MapsToGenericTypeDefinition(genericType)
              || givenType.HasInterfaceThatMapsToGenericTypeDefinition(genericType)
              || (givenType.BaseType != null && givenType.BaseType.IsAssignableToGenericType(genericType));
        }

        private static bool HasInterfaceThatMapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return givenType
              .GetInterfaces()
              .Where(it => it.IsGenericType)
              .Any(it => it.GetGenericTypeDefinition() == genericType);
        }

        private static bool MapsToGenericTypeDefinition(this Type givenType, Type genericType)
        {
            return genericType.IsGenericTypeDefinition
              && givenType.IsGenericType
              && givenType.GetGenericTypeDefinition() == genericType;
        }
    }
}
