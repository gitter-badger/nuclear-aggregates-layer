using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Security.UserContext.Profile;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
   public static class TimeZoneInfoExtenstion
    {
       public static DateTime ToUserDateTime(this DateTime time, LocaleInfo info)
       {
           return TimeZoneInfo.ConvertTimeFromUtc(time, info.UserTimeZoneInfo);
       }

       public static DateTime ToLocalUserTime(this LocaleInfo info)
       {
           return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, info.UserTimeZoneInfo);
       }
    }
}
