namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed class CardRelatedItemsJson
    {
        public string Name { get; set; }
        public string NameLocaleResourceId { get; set; }

        public string DisabledExpression { get; set; }
        public string LocalizedName { get; set; }
        public string Icon { get; set; }
        public bool IsCrmView { get; set; }
        public string RequestUrl { get; set; }
        public string ExtendedInfo { get; set; }
        public string AppendableEntity { get; set; }
    }
}
