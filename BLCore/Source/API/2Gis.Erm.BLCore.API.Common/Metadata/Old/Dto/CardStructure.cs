namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class CardStructure
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

        public ToolbarElementStructure[] CardToolbar { get; set; }
        public CardRelatedItemsGroupStructure[] CardRelatedItems { get; set; }

        // [JsonIgnore] fields
        public string CardNameLocaleResourceId { get; set; }
        public string EntityNameLocaleResourceId { get; set; }
    }
}