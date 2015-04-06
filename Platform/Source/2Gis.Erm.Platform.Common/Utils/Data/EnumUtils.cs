using System;

namespace DoubleGis.Erm.Platform.Common.Utils.Data
{
    public static class EnumUtils
    {
        public static TEnum ParseEnum<TEnum>(string value) where TEnum : struct, IConvertible
        {
            TEnum result;
            if (!TryParseEnum(value, out result))
            {
                throw new ArgumentException(string.Format("Can't parse enum value {0} of enum type {1}", value, typeof(TEnum)));
            }

            return result;
        }

        public static bool TryParseEnum<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
        {
            return Enum.TryParse(value, out result) && Enum.IsDefined(typeof(TEnum), result);
        }
    }
}