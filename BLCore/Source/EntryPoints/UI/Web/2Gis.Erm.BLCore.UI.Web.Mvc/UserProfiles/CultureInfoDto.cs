// ReSharper disable CheckNamespace

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class CultureInfoDto
    {
        public int LCID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTimeFormatInfoDto DateTimeFormat { get; set; }
        public NumberFormatInfoDto NumberFormat { get; set; }
        public string ThreeLetterISOLanguageName { get; set; }
        public string TwoLetterISOLanguageName { get; set; }
    }
}