using System;
using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticApi
    {
        T Get<T>(string id) where T : class;
        IReadOnlyCollection<IMultiGetHit<object>> MultiGet(Func<ErmMultiGetDescriptor, ErmMultiGetDescriptor> multiGetSelector);

        IDocumentWrapper<T> Create<T>(T @object, string id = null) where T : class;
        IDocumentWrapper<T> Update<T>(IDocumentWrapper<T> documentWrapper) where T : class;
        IDocumentWrapper<T> Delete<T>(IDocumentWrapper<T> documentWrapper) where T : class;

        ISearchResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class;
        IEnumerable<IDocumentWrapper<T>> Scroll<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher, IProgress<long> progress = null) where T : class;

        IEnumerable<IReadOnlyCollection<T>> CreateBatches<T>(IEnumerable<T> items);
        void Bulk(IReadOnlyCollection<Func<ErmBulkDescriptor, ErmBulkDescriptor>> selectors);

        void Refresh<T>() where T : class;
    }
}