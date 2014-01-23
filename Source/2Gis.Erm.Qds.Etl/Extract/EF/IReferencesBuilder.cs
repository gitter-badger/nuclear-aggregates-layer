namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IReferencesBuilder
    {
        void BuildReferences(IReferencesConsumer referencesConsumer);
    }
}