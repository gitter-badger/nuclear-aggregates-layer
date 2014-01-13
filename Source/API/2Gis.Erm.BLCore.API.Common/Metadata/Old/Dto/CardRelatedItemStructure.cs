namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
    public sealed class CardRelatedItemStructure
    {
        public string Name { get; set; }
        public string NameLocaleResourceId { get; set; }

        public string DisabledExpression { get; set; }
        public string LocalizedName { get; set; }
        public string Icon { get; set; }
        public bool IsCrmView { get; set; }
        public string RequestUrl { get; set; }
        public string FilterExpression { get; set; }
        public string ExtendedInfo { get; set; }
        public string AppendableEntity { get; set; }
    }
}
