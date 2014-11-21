using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed class NavigationDto
    {
        public string NameLocaleResourceId { get; set; }
        public string LocalizedName { get; set; }

        public string Icon { get; set; }
        public string RequestUrl { get; set; }

        public IEnumerable<NavigationDto> Items { get; set; }
    }
   
}