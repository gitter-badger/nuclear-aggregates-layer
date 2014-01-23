namespace DoubleGis.Erm.Qds
{
    public interface IDocsQueryBuilder
    {
        IDocsQuery CreateQuery(object entity);
    }
}