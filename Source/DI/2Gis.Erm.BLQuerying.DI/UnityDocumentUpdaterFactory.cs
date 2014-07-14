using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Elastic.Nest.Qds.Indexing;
using DoubleGis.Erm.Qds.Etl;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class UnityDocumentUpdaterFactory : IDocumentUpdaterFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IEntityToDocumentRelationMetadataContainer _metadataContainer;

        public UnityDocumentUpdaterFactory(IUnityContainer unityContainer, IEntityToDocumentRelationMetadataContainer metadataContainer)
        {
            _unityContainer = unityContainer;
            _metadataContainer = metadataContainer;
        }

        public IEnumerable<IDocumentUpdater> GetDocumentUpdatersForEntityType(Type entityType)
        {
            var metadatas = _metadataContainer.GetMetadatasForEntityType(entityType);
            var documentUpdaters = CreateDocumentUpdaters(metadatas);
            return documentUpdaters;
        }

        public IEnumerable<IDocumentUpdater> GetDocumentUpdatersForDocumentType(Type documentType)
        {
            var metadatas = _metadataContainer.GetMetadatasForDocumentType(documentType);
            var documentUpdaters = CreateDocumentUpdaters(metadatas);
            return documentUpdaters;
        }

        private IEnumerable<IDocumentUpdater> CreateDocumentUpdaters(IEnumerable<IEntityToDocumentRelationMetadata> metadatas)
        {
            var documentUpdaters = metadatas.Select(metadata =>
            {
                var resolveType = typeof(DocumentUpdater<,>).MakeGenericType(metadata.EntityType, metadata.DocumentType);
                return (IDocumentUpdater)_unityContainer.Resolve(resolveType);
            });

            return documentUpdaters;
        }
    }
}