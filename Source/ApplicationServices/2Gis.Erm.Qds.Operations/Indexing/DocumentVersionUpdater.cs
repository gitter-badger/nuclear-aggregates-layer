﻿using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Qds.Common;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentVersionUpdater<TDocument> : IDocumentVersionUpdater<TDocument>
        where TDocument : class
    {
        public Func<ElasticApi.MultiGetDescriptor2, ElasticApi.MultiGetDescriptor2> GetDocumentVersions(IReadOnlyCollection<IDocumentWrapper> documentWrappers)
        {
            var ids = documentWrappers.OfType<IDocumentWrapper<TDocument>>().Select(x => x.Id);

            return x => (ElasticApi.MultiGetDescriptor2)x.GetManyDistinct<TDocument>(ids).SourceEnabled2(false).Preference("_primary");
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