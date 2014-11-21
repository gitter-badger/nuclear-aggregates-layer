using System;
using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace DoubleGis.Erm.SqlClr
{
    public partial class ScalarFunctions
    {
        [SqlFunction(DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None, IsDeterministic = true)]
        public static SqlDateTime ConvertUtcToTimeZone(SqlDateTime utcDateTime, string timeZoneId)
        {
            return utcDateTime.IsNull ?
                utcDateTime :
                new SqlDateTime(TimeZoneInfo.ConvertTime(utcDateTime.Value, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId)));
        }

        [SqlFunction(DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None, IsDeterministic = true)]
        public static SqlString FormatPhoneAndFax(string phoneValue, string phoneFormat, string zoneValue)
        {
            var array = new char[phoneFormat.Length];

            var length = phoneValue.Length;
            for (var i = 0; i < phoneFormat.Length; i++)
            {
                var index = (phoneFormat.Length - i) - 1;
                if (char.IsDigit(phoneFormat[index]))
                {
                    array[index] = phoneValue[--length];
                }
                else
                {
                    array[index] = phoneFormat[index];
                }

                if (length == 0)
                {
                    break;
                }
            }

            var num4 = Array.LastIndexOf(array, '\0');
            phoneValue = new string(array, num4 + 1, (array.Length - num4) - 1);
            if (!string.IsNullOrEmpty(zoneValue))
            {
                phoneValue = string.Format("({0}) {1}", zoneValue, phoneValue);
            }

            return new SqlString(phoneValue);
        }
    }

}