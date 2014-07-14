using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.AcceptanceTests
{
    class RelatedEntityChangedContext<TEntity, TDoc> : EntityChangedContext<TEntity, TDoc>
        where TEntity : class, IEntityKey, IEntity, new()
        where TDoc : IDoc, new()
    {
        protected static void AddRelatedDocuments(string linkedFieldName, long id, params TDoc[] docs)
        {
            RelatedDocuments = docs;

            foreach (var doc in docs)
            {
                AddToDocsStorage(doc, linkedFieldName, id);
            }
        }

        protected static void PublishedDocsContainRelatedDocuments()
        {
            DocsStorage.NewPublishedDocs.Should().Contain(RelatedDocuments);
        }

        static TDoc[] RelatedDocuments;
    }
}