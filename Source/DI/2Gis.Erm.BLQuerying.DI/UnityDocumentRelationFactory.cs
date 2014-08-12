using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

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

        public IReadOnlyCollection<IDocumentVersionUpdater> GetDocumentVersionUpdaters(IEnumerable<Type> documentTypes)
        {
            var updaters = documentTypes.Select(x =>
            {
                var resolveType = typeof(IDocumentVersionUpdater<>).MakeGenericType(x);
                return (IDocumentVersionUpdater)_unityContainer.Resolve(resolveType);
            }).ToArray();

            return updaters;
        }

        public IReadOnlyCollection<IDocumentRelation> GetDocumentRelations(IEnumerable<Type> documentTypes)
        {
            var metadatas = _metadataContainer.GetMetadatasForDocumentType(documentTypes);
            var relations = metadatas.Select(x =>
            {
                var resolveType = typeof(IDocumentRelation<,>).MakeGenericType(x.Item1, x.Item2);
                return _unityContainer.Resolve(resolveType);
            });

            return relations.Cast<IDocumentRelation>().ToArray();
        }

        public IReadOnlyCollection<IDocumentPartRelation> GetDocumentPartRelations(IEnumerable<Type> documentTypes)
        {
            var metadatas = _metadataContainer.GetMetadatasForDocumentPartType(documentTypes);
            var relations = metadatas.Select(x =>
            {
                var resolveType = typeof(IDocumentRelation<,>).MakeGenericType(x.Item1, x.Item2);
                return _unityContainer.Resolve(resolveType);
            });

            return relations.Cast<IDocumentPartRelation>().ToArray();
        }
    }
}
