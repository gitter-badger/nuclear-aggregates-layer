using System;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class CalendarSettings
    {
        public enum StoreMode
        {
            Absolute = 0,
            Relative,
        }

        public enum DisplayMode
        {
            Day = 0,
            Month,
        }

        public StoreMode Store { get; set; }
        public DisplayMode Display { get; set; }

        public bool ReadOnly { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
    }
}