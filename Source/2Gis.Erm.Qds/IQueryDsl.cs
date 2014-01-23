namespace DoubleGis.Erm.Qds
{
    public interface IQueryDsl
    {
        IDocsQuery ByFieldValue(string docFieldName, object value);
    }
}