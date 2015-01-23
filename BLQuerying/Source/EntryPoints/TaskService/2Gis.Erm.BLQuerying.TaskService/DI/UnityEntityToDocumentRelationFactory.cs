using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;
using DoubleGis.Erm.Qds.Operations.Indexing;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Provider;

namespace DoubleGis.Erm.BLQuerying.TaskService.DI
{
    public sealed class UnityEntityToDocumentRelationFactory : IEntityToDocumentRelationFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IReadOnlyDictionary<Type, IEnumerable<IEntityRelationFeature>> _documentRelationMetadatas;

        public UnityEntityToDocumentRelationFactory(IUnityContainer unityContainer, IMetadataProvider metadataProvider)
        {
            _unityContainer = unityContainer;
            _documentRelationMetadatas = metadataProvider.GetEntityToDocumentProjectionMetadatas();
        }

        public IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForEntityType(Type entityType)
        {
            var applicableMetadatas = from metadata in _documentRelationMetadatas
                                      from entityRelationFeature in metadata.Value
                                      where entityRelationFeature.EntityType == entityType
                                      select metadata;

            return applicableMetadatas.Select(x => CreateEntityToDocumentRelations(x.Key, x.Value))
                                      .SelectMany(x => x)
                                      .ToArray();
        }

        public IReadOnlyCollection<IEntityToDocumentRelation> GetEntityToDocumentRelationsForDocumentType(Type documentType)
        {
            IEnumerable<IEntityRelationFeature> entityRelationFeatures;
            return _documentRelationMetadatas.TryGetValue(documentType, out entityRelationFeatures)
                       ? CreateEntityToDocumentRelations(documentType, entityRelationFeatures)
                       : new IEntityToDocumentRelation[0];
        }

        private IReadOnlyCollection<IEntityToDocumentRelation> CreateEntityToDocumentRelations(Type documentType, IEnumerable<IEntityRelationFeature> entityRelationFeatures)
        {
            return entityRelationFeatures
                .Select(feature =>
                            {
                                var relationType = typeof(IEntityToDocumentRelation<,>).MakeGenericType(feature.EntityType, documentType);
                                var featureType = typeof(EntityRelationFeature<,>).MakeGenericType(documentType, feature.EntityType);
                                return (IEntityToDocumentRelation)_unityContainer.Resolve(relationType, new DependencyOverride(featureType, feature));
                            })
                .ToArray();
        }
    }
}