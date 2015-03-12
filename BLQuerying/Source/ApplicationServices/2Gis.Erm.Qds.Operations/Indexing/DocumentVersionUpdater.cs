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
        public Func<ErmMultiGetDescriptor, ErmMultiGetDescriptor> GetDocumentVersions(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers)
        {
            var castedDocumentWrappers = documentWrappers.OfType<IDocumentWrapper<TDocument>>();

            return x => castedDocumentWrappers.Aggregate(x, (current, documentWrapper) => (ErmMultiGetDescriptor)current.GetDistinct<TDocument>(g => g
                .Source(s => s.Exclude("*"))
                .Id(documentWrapper.Id))
                .Preference("_primary"));
        }

        public void UpdateDocumentVersions(IReadOnlyCollection<IIndexedDocumentWrapper> documentWrappers, IReadOnlyCollection<IMultiGetHit<object>> hits)
        {
            var versionsMap = hits.OfType<IMultiGetHit<TDocument>>().Where(x => x.Found).ToDictionary(x => x.Id, x => long.Parse(x.Version));
            if (!versionsMap.Any())
            {
                return;
            }

            foreach (var documentWrapper in documentWrappers.OfType<IndexedDocumentWrapper<TDocument>>())
            {
                long version;
                if (!versionsMap.TryGetValue(documentWrapper.Id, out version))
                {
                    continue;
                }

                documentWrapper.Version = version;
            }
        }
    }
}