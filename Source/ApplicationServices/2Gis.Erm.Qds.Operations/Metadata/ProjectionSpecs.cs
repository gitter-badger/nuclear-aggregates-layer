namespace DoubleGis.Erm.Qds.Operations.Metadata
{
    public static partial class ProjectionSpecs
    {
        private static string GetRelatedId(long? relatedId)
        {
            return (relatedId == null) ? null : relatedId.Value.ToString();
        }
    }
}