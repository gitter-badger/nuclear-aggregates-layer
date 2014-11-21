using System;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class StringComparsionExtensions
    {
        public static bool EqualsIgnoreCase(this string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}