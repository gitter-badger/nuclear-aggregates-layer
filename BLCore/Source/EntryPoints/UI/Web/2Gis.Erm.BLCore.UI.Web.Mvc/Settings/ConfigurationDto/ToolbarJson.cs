namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
{
    public sealed partial class ToolbarJson
    {
        public string Name { get; set; }
        public string ParentName { get; set; }
        public string HideInCardRelatedGrid { get; set; }
        public bool DisableOnEmpty { get; set; }

        public string ControlType { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public bool Disabled { get; set; }
        public string LocalizedName { get; set; }
    }
}