// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto
// ReSharper restore CheckNamespace
{
    public sealed partial class CardJson
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public string EntityName { get; set; }
        public string EntityLocalizedName { get; set; }
        public string EntityMainAttribute { get; set; }
        public int? CrmEntityCode { get; set; }

        public bool HasComments { get; set; }
        public bool HasAdminTab { get; set; }
        public int DecimalDigits { get; set; }

        public ToolbarJson[] CardToolbar { get; set; }
        public CardRelatedItemsAreaJson[] CardRelatedItems { get; set; }
    }
}