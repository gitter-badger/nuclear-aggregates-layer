using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public sealed class UnityDocumentRelationFactory : IDocumentRelationFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IReadOnlyDictionary<Type, IEnumerable<IDocumentPartFeature>> _documentRelationMetadatas;

        public UnityDocumentRelationFactory(IUnityContainer unityContainer,
                                            IMetadataProvider metadataProvider)
        {
            _unityContainer = unityContainer;
            _documentRelationMetadatas = metadataProvider.GetDocumentRelationMetadatas();
        }

        public IReadOnlyCollection<IDocumentVersionUpdater> GetDocumentVersionUpdaters(IEnumerable<Type> documentTypes)
        {
            return documentTypes.Select(x =>
            {
                var resolveType = typeof(IDocumentVersionUpdater<>).MakeGenericType(x);
                return (IDocumentVersionUpdater)_unityContainer.Resolve(resolveType);
            })
            .ToArray();
        }

        public IReadOnlyCollection<IDocumentRelation> CreateDocumentRelations(IEnumerable<Type> documentTypes)
        {
            var relations = documentTypes
                .Where(x => _documentRelationMetadatas.ContainsKey(x))
                .Select(x => new
                {
                    DocumentType = x,
                    DocumentPartFeatures = _documentRelationMetadatas[x],
                })
                .SelectMany(x => ResolveDocumentRelations(x.DocumentType, x.DocumentPartFeatures))
                .Cast<IDocumentRelation>()
                .ToArray();
                
            return relations;
        }

        public IReadOnlyCollection<IDocumentPartRelation> CreateDocumentPartRelations(IEnumerable<Type> documentPartTypes)
        {
            var relations = _documentRelationMetadatas.Select(x => new
            {
                DocumentType = x.Key,
                DocumentPartFeatures = x.Value.Where(y => documentPartTypes.Contains(y.DocumentPartType)),
            })
            .Where(x => x.DocumentPartFeatures.Any())
            .SelectMany(x => ResolveDocumentRelations(x.DocumentType, x.DocumentPartFeatures))
            .Cast<IDocumentPartRelation>()
            .ToArray();

            return relations;
        }

        private IEnumerable<object> ResolveDocumentRelations(Type documentType, IEnumerable<IDocumentPartFeature> documentPartFeatures)
        {
            var relations = documentPartFeatures.GroupBy(x => x.DocumentPartType).Select(x =>
            {
                var documentPartType = x.Key;

                var resolveType = typeof(IDocumentRelation<,>).MakeGenericType(documentType, documentPartType);
                var accessorType = typeof(DocumentRelationAccessor<,>).MakeGenericType(documentType, documentPartType);
                var accessors = x.Select(y => (IDocumentRelationAccessor)Activator.CreateInstance(accessorType, y)).ToArray();

                var relation = _unityContainer.Resolve(resolveType, new DependencyOverride<IEnumerable<IDocumentRelationAccessor>>(accessors));
                return relation;
            });

            return relations;
        }
    }
}
