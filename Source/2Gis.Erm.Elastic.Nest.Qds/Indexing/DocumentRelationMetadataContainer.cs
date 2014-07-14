using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Etl;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Indexing
{
    public sealed class DocumentRelationMetadataContainer : IDocumentRelationMetadataContainer
    {
        private readonly Dictionary<Tuple<Type, Type>, object> _documentMetadataMap = new Dictionary<Tuple<Type, Type>, object>();

        public void RegisterMetadata<TDocument, TDocumentPart>(Func<IDocumentRelationMetadata<TDocument, TDocumentPart>> func)
        {
            var key = Tuple.Create(typeof(TDocument), typeof(TDocumentPart));

            _documentMetadataMap.Add(key, func);
        }

        public IDocumentRelationMetadata<TDocument, TDocumentPart> GetMetadata<TDocument, TDocumentPart>()
        {
            var key = Tuple.Create(typeof(TDocument), typeof(TDocumentPart));
            var func = (Func<IDocumentRelationMetadata<TDocument, TDocumentPart>>)_documentMetadataMap[key];

            var metadata = func();
            return metadata;
        }

        public IEnumerable<IDocumentRelationMetadata> GetDocumentMetadatas<TDocument>()
        {
            var metadatas = _documentMetadataMap.Where(x => x.Key.Item1 == typeof(TDocument)).Select(x => ((Func<IDocumentRelationMetadata>)x.Value)());
            return metadatas;
        }

        public IEnumerable<IDocumentPartRelationMetadata> GetDocumentPartMetadatas<TDocumentPart>()
        {
            var metadatas = _documentMetadataMap.Where(x => x.Key.Item2 == typeof(TDocumentPart)).Select(x => ((Func<IDocumentPartRelationMetadata>)x.Value)());
            return metadatas;
        }
    }
}