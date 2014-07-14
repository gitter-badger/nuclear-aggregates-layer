using System.Collections.Generic;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    // TODO {m.pashuk, 03.07.2014}: разобраться почему не используется TEntity
    public interface IEntityToDocumentRelation<TEntity, out TDocument>
    {
        IEnumerable<IDocumentWrapper<TDocument>> SelectAllDocuments();
        IEnumerable<IDocumentWrapper<TDocument>> SelectDocuments(ICollection<long> ids);
    }
}