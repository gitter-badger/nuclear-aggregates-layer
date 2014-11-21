using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed class CardRelatedItemsAreaJson
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string NameLocaleResourceId { get; set; }
        public IEnumerable<CardRelatedItemsJson> Items { get; set; }
    }
}