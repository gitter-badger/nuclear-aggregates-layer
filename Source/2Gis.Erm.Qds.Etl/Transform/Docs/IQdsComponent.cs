using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IQdsComponent
    {
        IDocsSelector CreateDocSelector();

        IDocRelation PartsDocRelation { get; }
        IDocRelation[] IndirectDocRelations { get; }

        IDoc CreateNewDoc(object part);
    }
}