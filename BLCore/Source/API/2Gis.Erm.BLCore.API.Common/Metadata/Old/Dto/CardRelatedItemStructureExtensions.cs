namespace DoubleGis.Erm.BLCore.API.Common.Metadata.Old.Dto
{
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