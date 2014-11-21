using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Collections;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting
{
    public static class FirmWorkingTimeLocalizer
    {
        private const string NewFormatPrefix = "{NF}";
        private static readonly DaysLocalizedNamesCache LocalizedDayNames = new DaysLocalizedNamesCache();

        public static string LocalizeWorkingTime(string firmWorkingTime, CultureInfo culture)
        {
            if (firmWorkingTime == null)
            {
                return null;
            }

            var localizedDayNames = LocalizedDayNames.GetOrAdd(culture);
            var sb = new StringBuilder(firmWorkingTime.Length * 2);

            var days = firmWorkingTime.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string dayString in days)
            {
                if (string.IsNullOrWhiteSpace(dayString))
                {
                    continue;
                }

                // Время работы в старом формате.
                if (!dayString.StartsWith(NewFormatPrefix))
                {
                    sb.Append(dayString.Trim());
                    sb.Append("; ");
                    continue;
                }

                var parts = dayString.Replace(NewFormatPrefix, string.Empty).Split('|');
                var localizedDayName = localizedDayNames[parts[0]];

                sb.Append(localizedDayName);
                sb.Append(": ");

                if (string.IsNullOrEmpty(parts[1]))
                {
                    sb.Append(BLResources.ResourceManager.GetString(StaticReflection.GetMemberName(() => BLResources.NotWorkingDay), culture));
                }
                else if (parts[1] == "00:00 - 00:00")
                {
                    sb.Append(BLResources.ResourceManager.GetString(StaticReflection.GetMemberName(() => BLResources.RoundTheClock), culture));
                }
                else
                {
                    sb.Append(parts[1]);
                }

                if (parts.Length > 2)
                {
                    sb.Append(", ");
                    sb.Append(BLResources.ResourceManager.GetString(StaticReflection.GetMemberName(() => BLResources.Lunch), culture));
                    sb.Append(" ");
                    sb.Append(parts[2]);
                }

                sb.Append("; ");
            }

            return sb.ToString();
        }

        private static string MakeFirstCharUpper(string s)
        {
            if (s.Length <= 1)
            {
                return s.ToUpper();
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private sealed class DaysLocalizedNamesCache : Registry<CultureInfo, Dictionary<string, string>>
        {
            protected override Dictionary<string, string> CreateValue(CultureInfo culture)
            {
                return new Dictionary<string, string>
                    {
                        { "Mon", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Monday)) },
                        { "Tue", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday)) },
                        { "Wed", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday)) },
                        { "Thu", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Thursday)) },
                        { "Fri", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Friday)) },
                        { "Sat", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Saturday)) },
                        { "Sun", MakeFirstCharUpper(culture.DateTimeFormat.GetDayName(DayOfWeek.Sunday)) }
                    };
            }
        }
    }
}
