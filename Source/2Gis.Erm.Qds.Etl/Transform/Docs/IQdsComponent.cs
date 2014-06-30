using DoubleGis.Erm.Qds.Etl.Transform.EF;

namespace DoubleGis.Erm.Qds.Etl.Transform.Docs
{
    public interface IQdsComponent
    {
        IDocsUpdater CreateDocUpdater();

        IDocRelation PartsDocRelation { get; }

        IDoc CreateNewDoc(object part);
    }
}