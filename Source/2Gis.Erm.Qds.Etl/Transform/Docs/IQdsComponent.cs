using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IQdsComponent
    {
        IDocsUpdater CreateDocUpdater();

        IDocRelation PartsDocRelation { get; }
        IDocRelation[] IndirectDocRelations { get; }

        IDoc CreateNewDoc(object part);
    }
}