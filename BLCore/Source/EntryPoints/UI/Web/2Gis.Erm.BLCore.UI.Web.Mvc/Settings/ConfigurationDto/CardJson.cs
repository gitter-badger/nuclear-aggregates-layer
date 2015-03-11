// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
// ReSharper restore CheckNamespace
{
    public sealed partial class CardJson
    {
        public string Icon { get; set; }
        public string LargeIcon { get; set; }

        public string CardLocalizedName { get; set; }

        public string EntityName { get; set; }
        public string EntityLocalizedName { get; set; }
        public string EntityMainAttribute { get; set; }

        public bool HasComments { get; set; }
        public bool HasAdminTab { get; set; }
        public bool HasActionsHistory { get; set; }
        public int DecimalDigits { get; set; }

        public ToolbarJson[] CardToolbar { get; set; }
        public CardRelatedItemsAreaJson[] CardRelatedItems { get; set; }
    }
}