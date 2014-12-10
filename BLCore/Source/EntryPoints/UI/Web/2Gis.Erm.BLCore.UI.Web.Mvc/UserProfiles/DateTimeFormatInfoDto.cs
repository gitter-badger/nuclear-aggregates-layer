// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class DateTimeFormatInfoDto
    {
        public string[] DayNames { get; set; }

        public int FirstDayOfWeek { get; set; }

        public string[] MonthNames { get; set; }
        
        public int TimeOffsetInMinutes { get; set; }

        public string TimeZoneId { get; set; }

        public string DotNetShortDatePattern { get; set; }

        public string DotNetShortTimePattern { get; set; }

        public string DotNetFullDateTimePattern { get; set; }

        public string DotNetInvariantDateTimePattern { get; set; }

        public string PhpShortDatePattern { get; set; }

        public string PhpShortTimePattern { get; set; }

        public string PhpFullDateTimePattern { get; set; }

        public string PhpInvariantDateTimePattern { get; set; }

        public string PhpYearMonthPattern { get; set; }
        
        public string DotNetYearMonthPattern { get; set; }
    }
}