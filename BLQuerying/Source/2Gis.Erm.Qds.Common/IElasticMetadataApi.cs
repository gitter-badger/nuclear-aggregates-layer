namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticMetadataApi
    {
        void RegisterType<T>(string docIndexName, string docTypeName = null);
    }
}