using System;
using System.Globalization;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using NuClear.Security.API.UserContext.Profile;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
{
    public static class UserLocaleInfoExtensions
    {
        private static readonly string[] CurrencyNegativePatterns = { "($n)", "-$n", "$-n", "$n-", "(n$)", "-n$", "n-$", "n$-", "-n $", "-$ n", "n $-", "$ n-", "$ -n", "n- $", "($ n)", "(n $)" };
        private static readonly string[] CurrencyPositivePatterns = { "$n", "n$", "$ n", "n $" };
        private static readonly string[] NumberNegativePatterns = { "(n)", "-n", "- n", "n-", "n -" };

        public static int ToOffsetFromUtcInMinutes(this TimeZoneInfo timeZoneInfo)
        {
            var utcTime = DateTime.UtcNow;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
            return Convert.ToInt32(localTime.Subtract(utcTime).TotalMinutes);
        }

        public static UserLocaleInfo ToUserLocalInfo(this IUserProfile ermUserProfile)
        {
            return new UserLocaleInfo(  ermUserProfile.UserLocaleInfo.UserCultureInfo.Name,
                                        ermUserProfile.UserLocaleInfo.UserCultureInfo.TwoLetterISOLanguageName, 
                                        ermUserProfile.UserLocaleInfo.UserCultureInfo.NumberFormat, 
                                        ermUserProfile.UserLocaleInfo.UserCultureInfo.DateTimeFormat,
                                        ermUserProfile.UserLocaleInfo.UserTimeZoneInfo.ToOffsetFromUtcInMinutes(),
                                        ermUserProfile.UserLocaleInfo.UserTimeZoneInfo.Id);   
        }

        public static UserLocaleInfo GetUserLocaleInfo(this ViewDataDictionary viewData)
        {
            object userLocaleInfo;
            if (!viewData.TryGetValue(UserLocaleInfo.UserLocaleInfoKey, out userLocaleInfo))
            {
                throw new NotificationException(BLResources.UserLocaleInfoNotFound);
            }

            return (UserLocaleInfo)userLocaleInfo;
        }

        public static NumberFormatInfoDto ToDto(this NumberFormatInfo numberFormatInfo)
        {
            return new NumberFormatInfoDto
            {
                CurrencyDecimalDigits = numberFormatInfo.CurrencyDecimalDigits,
                CurrencyDecimalSeparator = numberFormatInfo.CurrencyDecimalSeparator,
                CurrencyGroupSeparator = numberFormatInfo.CurrencyGroupSeparator,
                CurrencyGroupSizes = numberFormatInfo.CurrencyGroupSizes,
                CurrencyNegativePattern = CurrencyNegativePatterns[numberFormatInfo.CurrencyNegativePattern],
                CurrencyPositivePattern = CurrencyPositivePatterns[numberFormatInfo.CurrencyPositivePattern],
                CurrencySymbol = numberFormatInfo.CurrencySymbol,
                NegativeInfinitySymbol = numberFormatInfo.NegativeInfinitySymbol,
                NegativeSign = numberFormatInfo.NegativeSign,
                NumberDecimalDigits = numberFormatInfo.NumberDecimalDigits,
                NumberDecimalSeparator = numberFormatInfo.NumberDecimalSeparator,
                NumberGroupSeparator = numberFormatInfo.NumberGroupSeparator,
                NumberGroupSizes = numberFormatInfo.NumberGroupSizes,
                NumberNegativePattern = NumberNegativePatterns[numberFormatInfo.NumberNegativePattern],
                PositiveInfinitySymbol = numberFormatInfo.PositiveInfinitySymbol,
                PositiveSign = numberFormatInfo.PositiveSign        
            };
        }

        public static DateTimeFormatInfoDto ToDto(this DateTimeFormatInfo dateTimeFormatInfo, int timeOffsetInMinutes, string timeZoneId)
        {
            return new DateTimeFormatInfoDto
            {
                DayNames = dateTimeFormatInfo.DayNames,
                FirstDayOfWeek = (int)dateTimeFormatInfo.FirstDayOfWeek,
                MonthNames = dateTimeFormatInfo.MonthNames,
                TimeOffsetInMinutes = timeOffsetInMinutes,
                TimeZoneId = timeZoneId,
                DotNetShortDatePattern = dateTimeFormatInfo.ShortDatePattern,
                DotNetShortTimePattern = dateTimeFormatInfo.ShortTimePattern,
                //Делаем так потому, что все полные форматы в дотнет выводят месяц названием.
                DotNetFullDateTimePattern = dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.LongTimePattern,
                DotNetInvariantDateTimePattern = "MM/dd/yyyy HH:mm:ss",
                DotNetYearMonthPattern = dateTimeFormatInfo.YearMonthPattern,
                PhpShortDatePattern = DateTimeExtensions.ConvertNetToPhp(dateTimeFormatInfo.ShortDatePattern),
                PhpShortTimePattern = DateTimeExtensions.ConvertNetToPhp(dateTimeFormatInfo.ShortTimePattern),
                //Делаем так потому, что все полные форматы в дотнет выводят месяц названием.
                PhpFullDateTimePattern = DateTimeExtensions.ConvertNetToPhp(dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.LongTimePattern),
                PhpInvariantDateTimePattern = DateTimeExtensions.ConvertNetToPhp("MM/dd/yyyy HH:mm:ss"),        
                PhpYearMonthPattern = DateTimeExtensions.ConvertNetToPhp(dateTimeFormatInfo.YearMonthPattern), // todo разделить php и dotnet версии, чтобы по названию было четко ясно что за версию шаблона используем

                MomentJsShortDatePattern = DateTimeExtensions.ConvertNetToMomentJs(dateTimeFormatInfo.ShortDatePattern),
                MomentJsYearMonthPattern = DateTimeExtensions.ConvertNetToMomentJs(dateTimeFormatInfo.YearMonthPattern),
                MomentJsShortTimePattern = DateTimeExtensions.ConvertNetToMomentJs(dateTimeFormatInfo.ShortTimePattern),
            };
        }
    }
}