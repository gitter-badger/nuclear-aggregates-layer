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
        public string ExtendedInfo { get; set; }
        public string AppendableEntity { get; set; }
    }

    public static class CardRelatedItemStructureExtensions
    {
        public static void AppendExtendedInfo(this CardRelatedItemStructure structure, string extendedInfo)
        {
            if (string.IsNullOrWhiteSpace(structure.ExtendedInfo))
            {
                structure.ExtendedInfo = extendedInfo;
            }
            else
            {
                structure.ExtendedInfo += string.Format("&{0}", extendedInfo);
            }
        }

        public static void AppendDisabledExpression(this CardRelatedItemStructure structure, string expression)
        {
            if (string.IsNullOrWhiteSpace(structure.DisabledExpression))
            {
                structure.DisabledExpression = expression;
            }
            else
            {
                structure.DisabledExpression += string.Format("||({0})", expression);
            }
        }
    }
}
