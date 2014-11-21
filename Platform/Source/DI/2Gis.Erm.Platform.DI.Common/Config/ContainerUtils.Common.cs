using System;

namespace DoubleGis.Erm.Platform.DI.Common.Config
{
    public static partial class ContainerUtils
    {
        public static string GetPerCallUniqueMarker()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetPerTypeUniqueMarker(this Type sourceType)
        {
            return sourceType.AssemblyQualifiedName;
        }

        public static string GetPerTypeUniqueMarker<T>()
        {
            return typeof(T).GetPerTypeUniqueMarker();
        }
    }
}
