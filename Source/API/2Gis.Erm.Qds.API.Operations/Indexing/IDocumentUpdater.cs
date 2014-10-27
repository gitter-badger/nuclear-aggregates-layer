using System;
using System.Collections.Generic;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IDocumentUpdater
    {
        void IndexDocuments(IReadOnlyCollection<EntityLink> entityLinks);
        void IndexAllDocuments(Type documentType, CancellationToken cancellationToken, IProgress<long> countProgress = null, IProgress<long> totalCountProgress = null);
    }
}