using System;

namespace DoubleGis.Erm.BLCore.UI.Web.Silverlight.Infrastructure.Interaction
{
    public static class ReflectionUtils
    {
        public static bool IsNullable(this Type checkingType)
        {
            return checkingType.IsGenericType && checkingType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
