namespace DoubleGis.Erm.Qds
{
    // TODO {f.zaharov, 04.06.2014}: Надо подумать над этой функциональностью, вероятно она слишком расслоена
    public interface IQueryDsl
    {
        IDocsQuery ByFieldValue(string docFieldName, object value);
        IDocsQuery ByNestedObjectQuery(string nestedObjectName, IDocsQuery nestedQuery);
        IDocsQuery Or(IDocsQuery leftQuery, IDocsQuery rightQuery);
    }
}