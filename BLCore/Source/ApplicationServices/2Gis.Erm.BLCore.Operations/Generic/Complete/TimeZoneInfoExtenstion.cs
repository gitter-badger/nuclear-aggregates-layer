using System;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
   public static class TimeZoneInfoExtenstion
    {      
       public static DateTime ConvertDate(this TimeZoneInfo from, TimeZoneInfo to, DateTime date)
       {
           return TimeZoneInfo.ConvertTime(date, from, to);
       }

       public static DateTime ConvertDateFromUtc(this TimeZoneInfo toTimeZoneInfo, DateTime date)
       {
           return ConvertDate(TimeZoneInfo.Utc, toTimeZoneInfo, date);
       }
       
       public static DateTime ConvertDateFromLocal(this TimeZoneInfo toTimeZoneInfo, DateTime date)
       {
           return ConvertDate(TimeZoneInfo.Local, toTimeZoneInfo, date);
       }
    }
}
