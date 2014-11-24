using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime UpdateKindIfUnset(this DateTime dateTime, DateTimeKind dateTimeKind = DateTimeKind.Utc)
        {
            return dateTime.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dateTime, dateTimeKind) : dateTime;
        }

        public static DateTime TrimToSeconds(this DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % TimeSpan.TicksPerSecond), dateTime.Kind);
        }

        public static DateTime GetFirstDateOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
        }

        public static DateTime GetPrevMonthFirstDate(this DateTime dateTime)
        {
            var nextMonthDate = dateTime.AddMonths(-1);
            return new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1, 0, 0, 0);
        }

        public static DateTime GetPrevMonthLastDate(this DateTime dateTime)
        {
            var nextMonthDate = dateTime.AddMonths(-1);
            return GetEndPeriodOfThisMonth(nextMonthDate);
        }

        public static DateTime GetNextMonthFirstDate(this DateTime dateTime)
        {
            var nextMonthDate = dateTime.AddMonths(1);
            return new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1, 0, 0, 0);
        }

        public static DateTime GetNextMonthLastDate(this DateTime dateTime)
        {
            var nextMonthDate = dateTime.AddMonths(1);
            return GetEndPeriodOfThisMonth(nextMonthDate);
        }

        public static DateTime GetEndPeriodOfThisMonth(this DateTime dateTime)
        {
            return dateTime.GetNextMonthFirstDate().AddSeconds(-1);
        }

        public static DateTime GetLastDateOfMonth(this DateTime dateTime)
        {
            var firstDateOfMonth = GetFirstDateOfMonth(dateTime);
            return firstDateOfMonth.AddMonths(1).AddSeconds(-1);
        }

        public static DateTime GetEndOfTheDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddSeconds(-1);
        }

        public static int MonthDifference(this DateTime startDate, DateTime endDate)
        {
            return Math.Abs(12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month);
        }

        /// <summary>
        /// Accepts a Unix/PHP date string format and returns a valid .NET date format
        /// </summary>
        /// <param name="format">The PHP format string</param>
        /// <returns>The format string converted to .NET DateTime format specifiers</returns>
        public static string ConvertPhpToNet(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            var final = new StringBuilder(128);
            var m = Regex.Match(format, @"(%|\\)?.|%%", RegexOptions.IgnoreCase);

            while (m.Success)
            {
                string temp = m.Value;

                if (temp.StartsWith(@"\") || temp.StartsWith("%%"))
                {
                    final.Append(temp.Replace(@"\", string.Empty).Replace("%%", "%"));
                }

                switch (temp)
                {
                    case "d":
                        final.Append("dd");
                        break;
                    case "D":
                        final.Append("ddd");
                        break;
                    case "j":
                        final.Append("d");
                        break;
                    case "l":
                        final.Append("dddd");
                        break;
                    case "F":
                        final.Append("MMMM");
                        break;
                    case "m":
                        final.Append("MM");
                        break;
                    case "M":
                        final.Append("MMM");
                        break;
                    case "n":
                        final.Append("M");
                        break;
                    case "Y":
                        final.Append("yyyy");
                        break;
                    case "y":
                        final.Append("yy");
                        break;
                    case "a":
                    case "A":
                        final.Append("tt");
                        break;
                    case "g":
                        final.Append("h");
                        break;
                    case "G":
                        final.Append("H");
                        break;
                    case "h":
                        final.Append("hh");
                        break;
                    case "H":
                        final.Append("HH");
                        break;
                    case "i":
                        final.Append("mm");
                        break;
                    case "s":
                        final.Append("ss");
                        break;
                    default:
                        final.Append(temp);
                        break;
                }

                m = m.NextMatch();
            }

            return final.ToString();
        }

        /// <summary>
        /// Accepts a Unix/PHP date string format and returns a valid .NET date format
        /// </summary>
        /// <param name="format">The .NET format string</param>
        /// <returns>The format string converted to PHP format specifiers</returns>
        public static string ConvertNetToPhp(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            var final = new StringBuilder(128);

            switch (format.Trim())
            {
                case "d":
                    format = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    break;
                case "D":
                    format = CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
                    break;
                case "t":
                    format = CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
                    break;
                case "T":
                    format = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
                    break;
            }

            var m = Regex.Match(format, @"(\\)?(dd?d?d?|MM?M?M?|yy?y?y?|hh?|HH?|mm?|ss?|tt?|S)|.", RegexOptions.IgnoreCase);

            while (m.Success)
            {
                string temp = m.Value;

                switch (temp)
                {
                    case "dd":
                        final.Append("d");
                        break;
                    case "ddd":
                        final.Append("D");
                        break;
                    case "d":
                        final.Append("j");
                        break;
                    case "dddd":
                        final.Append("l");
                        break;
                    case "MMMM":
                        final.Append("F");
                        break;
                    case "MM":
                        final.Append("m");
                        break;
                    case "MMM":
                        final.Append("M");
                        break;
                    case "M":
                        final.Append("n");
                        break;
                    case "yyyy":
                        final.Append("Y");
                        break;
                    case "yy":
                        final.Append("y");
                        break;
                    case "tt":
                        final.Append("a");
                        break;
                    case "h":
                        final.Append("g");
                        break;
                    case "H":
                        final.Append("G");
                        break;
                    case "hh":
                        final.Append("h");
                        break;
                    case "HH":
                        final.Append("H");
                        break;
                    case "mm":
                        final.Append("i");
                        break;
                    case "ss":
                        final.Append("s");
                        break;
                    default:
                        final.Append(temp);
                        break;
                }

                m = m.NextMatch();
            }

            return final.ToString();
        }

        public static DateTime AddDaysWithDayOffs(this DateTime dateTime, int days)
        {
            var result = dateTime;

            // move to first workday
            while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = result.AddDays(1);
            }

            // add days
            for (var i = 0; i < days; i++)
            {
                result = result.AddDays(1);
                while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
                {
                    result = result.AddDays(1);
                }
            }

            return result;
        }
    }
}