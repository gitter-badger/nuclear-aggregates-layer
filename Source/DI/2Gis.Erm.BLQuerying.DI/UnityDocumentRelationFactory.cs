using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Qds.Etl;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class UnityDocumentRelationFactory : IDocumentRelationFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDocumentRelationMetadataContainer _metadataContainer;

        public UnityDocumentRelationFactory(IUnityContainer unityContainer, IDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public IEnumerable<IDocumentRelation<TDocument>> GetDocumentRelations<TDocument>()
        {
            var metadatas = _metadataContainer.GetDocumentMetadatas<TDocument>();

            var relations = metadatas.Select(x =>
            {
                var resolveType = typeof(DocumentRelation<,>).MakeGenericType(typeof(TDocument), x.DocumentPartType);
                return _unityContainer.Resolve(resolveType);
            });

            return relations.Cast<IDocumentRelation<TDocument>>();
        }

        public IEnumerable<IDocumentPartRelation<TDocumentPart>> GetDocumentPartRelations<TDocumentPart>()
        {
            var metadatas = _metadataContainer.GetDocumentPartMetadatas<TDocumentPart>();
            var relations = metadatas.Select(x =>
            {
                var resolveType = typeof(DocumentRelation<,>).MakeGenericType(x.DocumentType, typeof(TDocumentPart));
                return _unityContainer.Resolve(resolveType);
            });

            return relations.Cast<IDocumentPartRelation<TDocumentPart>>();
        }
    }
}
