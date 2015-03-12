using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Indexing.Metadata.Features;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata;
using DoubleGis.Erm.Qds.API.Operations.Replication.Metadata.Features;

namespace DoubleGis.Erm.Qds.API.Operations
{
    public static class MetadataProviderExtensions
    {
        public static IReadOnlyDictionary<Type, IEnumerable<IDocumentPartFeature>> GetDocumentRelationMetadatas(this IMetadataProvider metadataProvider)
        {
            MetadataSet metadataSet;
            var metadatas = metadataProvider.TryGetMetadata<DocumentIndexingIdentity>(out metadataSet)
                                ? metadataSet.Metadata.Values.Cast<DocumentIndexingMetadata>().ToList()
                                : Enumerable.Empty<DocumentIndexingMetadata>();
            return metadatas.ToDictionary(x => x.DocumentType, x => x.Features.OfType<IDocumentPartFeature>());
        }

        public static IReadOnlyDictionary<Type, IEnumerable<IEntityRelationFeature>> GetEntityToDocumentProjectionMetadatas(this IMetadataProvider metadataProvider)
        {
            MetadataSet metadataSet;
            var metadatas = metadataProvider.TryGetMetadata<EntityToDocumentProjectionsIdentity>(out metadataSet)
                                ? metadataSet.Metadata.Values.Cast<EntityToDocumentProjectionMetadata>()
                                : Enumerable.Empty<EntityToDocumentProjectionMetadata>();
            return metadatas.ToDictionary(x => x.DocumentType, x => x.Features.OfType<IEntityRelationFeature>());
        }
    }
}