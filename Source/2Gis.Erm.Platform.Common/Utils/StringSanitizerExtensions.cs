using System;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;

namespace DoubleGis.Erm.Common.Utils
{
    // FIXME {d.ivanov, 26.11.2013}: ляжет в 2Gis.Erm.Platform.Common\Utils\StringSanitizerExtensions.cs
    public static class StringSanitizerExtensions
    {
        private const char NonBreakingSpace = (char)160;
        private const char Space = (char)32;
        private const char LineFeed = (char)10;
        private const char CarriageReturn = (char)13;
        private const char Tabulation = (char)9;

        private static readonly Func<char, bool> ForbiddenCharacter = c => char.IsControl(c)
                                                                  && c != LineFeed && c != Tabulation // управляющие, но не запрещённые
                                                                  && c != NonBreakingSpace && c != CarriageReturn; // управляющие, но автоматически исправляемые

        public static string EnsureСleanness(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (value.Any(ForbiddenCharacter))
            {
                throw new Exception(BLResources.StringContainsControlCharacters);
            }

            return value.Replace(NonBreakingSpace, Space)
                        .Replace(string.Concat(CarriageReturn, LineFeed), LineFeed.ToString())
                        .Replace(CarriageReturn.ToString(), string.Empty);
        }

        public static bool ContainForbiddenCharacters(this string value)
        {
            return (value ?? string.Empty).Any(ForbiddenCharacter);
        }
    }
}
