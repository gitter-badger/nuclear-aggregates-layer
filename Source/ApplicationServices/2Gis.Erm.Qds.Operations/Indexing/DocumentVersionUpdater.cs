using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentVersionUpdater<TDocument> : IDocumentVersionUpdater<TDocument>
        where TDocument : class
    {
        public Func<ElasticApi.ErmMultiGetDescriptor, ElasticApi.ErmMultiGetDescriptor> GetDocumentVersions(IReadOnlyCollection<IDocumentWrapper> documentWrappers)
        {
            var castedDocumentWrappers = documentWrappers.OfType<IDocumentWrapper<TDocument>>();

            return x => castedDocumentWrappers.Aggregate(x, (current, documentWrapper) => (ElasticApi.ErmMultiGetDescriptor)current.GetDistinct<TDocument>(g => g
                .Source(s => s.Exclude("*"))
                .Id(documentWrapper.Id))
                .Preference("_primary"));
        }

        public void UpdateDocumentVersions(IReadOnlyCollection<IDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits)
        {
            var versionsMap = hits.OfType<IMultiGetHit<TDocument>>().Where(x => x.Found).ToDictionary(x => x.Id, x => x.Version);
            if (!versionsMap.Any())
            {
                return;
            }

            foreach (var documentWrapper in documentWrappers.OfType<DocumentWrapper<TDocument>>())
            {
                string version;
                if (!versionsMap.TryGetValue(documentWrapper.Id, out version))
                {
                    continue;
                }

                documentWrapper.Version = version;
            }
        }
    }
}