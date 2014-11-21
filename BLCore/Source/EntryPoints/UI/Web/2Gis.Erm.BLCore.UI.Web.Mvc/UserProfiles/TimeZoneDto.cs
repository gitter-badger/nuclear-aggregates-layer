using System;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class TimeZoneDto
    {
        public long Id { get; set; }
        public TimeSpan BaseUtcOffset { get; set; }
        public string DaylightName { get; set; }
        public string DisplayName { get; set; }
        public string TimeZoneId { get; set; }
        public string StandardName { get; set; }
        public bool SupportsDaylightSavingTime { get; set; }
    }
}