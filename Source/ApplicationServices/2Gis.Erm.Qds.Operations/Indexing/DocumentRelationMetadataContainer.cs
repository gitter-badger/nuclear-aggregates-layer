using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Indexing;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentRelationMetadataContainer : IDocumentRelationMetadataContainer
    {
        private readonly Dictionary<IDocumentRelationMetadata, Tuple<Type, Type>> _documentMetadataMap = new Dictionary<IDocumentRelationMetadata, Tuple<Type, Type>>();

        public void RegisterMetadata<TDocument, TDocumentPart>(IDocumentRelationMetadata metadata)
        {
            var value = Tuple.Create(typeof(TDocument), typeof(TDocumentPart));
            _documentMetadataMap.Add(metadata, value);
        }

        public IEnumerable<IDocumentRelationMetadata<TDocument, TDocumentPart>> GetMetadatas<TDocument, TDocumentPart>()
        {
            var value = (IStructuralEquatable)Tuple.Create(typeof(TDocument), typeof(TDocumentPart));
            var metadatas = _documentMetadataMap
                .Where(x => value.Equals(x.Value))
                .Select(x => (IDocumentRelationMetadata<TDocument, TDocumentPart>)x.Key);
            return metadatas;
        }

        public IEnumerable<Tuple<Type, Type>> GetMetadatasForDocumentType(IEnumerable<Type> documentTypes)
        {
            var metadatas = _documentMetadataMap
                .Where(x => documentTypes.Contains(x.Value.Item1))
                .Select(x => x.Value)
                .Distinct();
            return metadatas;
        }

        public IEnumerable<Tuple<Type, Type>> GetMetadatasForDocumentPartType(IEnumerable<Type> documentPartTypes)
        {
            var metadatas = _documentMetadataMap
                .Where(x => documentPartTypes.Contains(x.Value.Item2))
                .Select(x => x.Value)
                .Distinct();
            return metadatas;
        }
    }
}