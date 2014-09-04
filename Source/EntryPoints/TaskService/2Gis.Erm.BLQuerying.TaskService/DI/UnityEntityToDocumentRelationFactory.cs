using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public sealed class UnityEntityToDocumentRelationFactory : IEntityToDocumentRelationFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public UnityEntityToDocumentRelationFactory(IUnityContainer unityContainer, IEntityToDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForEntityType(Type entityType)
        {
            var metadatas = _metadataContainer.GetMetadatasForEntityType(entityType);
            var relations = CreateEntityToDocumentRelations(metadatas);
            return relations;
        }

        public IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForDocumentType(Type documentType)
        {
            var metadatas = _metadataContainer.GetMetadatasForDocumentType(documentType);
            var relations = CreateEntityToDocumentRelations(metadatas);
            return relations;
        }

        private IReadOnlyCollection<IEntityToDocumentRelation> CreateEntityToDocumentRelations(IEnumerable<IEntityToDocumentRelationMetadata> metadatas)
        {
            var relations = metadatas.Select(metadata =>
            {
                var resolveType = typeof(IEntityToDocumentRelation<,>).MakeGenericType(metadata.EntityType, metadata.DocumentType);
                return (IEntityToDocumentRelation)_unityContainer.Resolve(resolveType);
            });

            return relations.ToArray();
        }
    }
}