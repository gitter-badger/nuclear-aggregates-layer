using System;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public sealed class CalendarSettings
    {
        public enum StoreMode
        {
            /// <summary>
            /// Точный момент времени. Неизменно фактическое значение, но пользовательское представление от пояса к поясу может меняться.
            /// </summary>
            Absolute = 0,

            /// <summary>
            /// Относительное время. Отображается для всех часовых поясов одинаково и не представляет какого-либо точного времени (даже если содержит часы-минуты)
            /// </summary>
            Relative,
        }

        public enum DisplayMode
        {
            Day = 0,
            Month,
        }        

        public StoreMode Store { get; set; }
        public DisplayMode Display { get; set; }
        public TimeSettings Time { get; set; }

        public bool ReadOnly { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }

        public class TimeSettings
        {
            public static readonly TimeSettings WorkHours = new TimeSettings(TimeSpan.FromHours(8), TimeSpan.FromHours(20), TimeSpan.FromMinutes(30));

            public TimeSettings()
            {
                Start = TimeSpan.FromHours(0);
                End = TimeSpan.FromDays(1).Add(TimeSpan.FromSeconds(-1));
                Step = TimeSpan.FromMinutes(30);
            }

            public TimeSettings(TimeSpan start, TimeSpan end, TimeSpan step)
            {
                End = end;
                Start = start;
                Step = step;
            }           

            public TimeSpan Start { get; private set; }
            public TimeSpan End { get; private set; }
            public TimeSpan Step { get; private set; }
        }
    }
}