namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class CardStructure
    {
        public string Icon { get; set; }
        public string LargeIcon { get; set; }

        public string Title { get; set; }

        public string EntityName { get; set; }
        public string EntityLocalizedName { get; set; }
        public string EntityMainAttribute { get; set; }
        public int? CrmEntityCode { get; set; }

        public bool HasComments { get; set; }
        public bool HasAdminTab { get; set; }

        // COMMENT {all, 20.11.2014}: Есть мнение, что этому тут не место
        public int DecimalDigits { get; set; }

        public ToolbarElementStructure[] CardToolbar { get; set; }
        public CardRelatedItemsGroupStructure[] CardRelatedItems { get; set; }

        // [JsonIgnore] fields
        public string TitleResourceId { get; set; }
        public string EntityNameLocaleResourceId { get; set; }
    }
}