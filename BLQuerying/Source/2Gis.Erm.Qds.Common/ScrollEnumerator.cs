using System;
using System.Collections;
using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    internal sealed class ScrollEnumerator<TDocument> : IEnumerator<IDocumentWrapper<TDocument>>
        where TDocument : class
    {
        private readonly IElasticClient _elasticClient;
        private readonly Func<ISearchResponse<TDocument>> _searchFunc;
        private readonly string _scrollTimeout;
        private readonly IProgress<long> _progress;

        private string _scrollId;
        private IEnumerator<IHit<TDocument>> _internalEnumerator;

        public ScrollEnumerator(IElasticClient elasticClient, Func<ISearchResponse<TDocument>> searchFunc, string scrollTimeout, IProgress<long> progress = null)
        {
            _elasticClient = elasticClient;
            _searchFunc = searchFunc;
            _scrollTimeout = scrollTimeout;
            _progress = progress;
        }

        public IDocumentWrapper<TDocument> Current
        {
            get
            {
                var current = _internalEnumerator.Current;
                return new DocumentWrapper<TDocument>
                           {
                               Id = current.Id,
                               Document = current.Source,
                               Version = current.Version == null ? (long?)null : long.Parse(current.Version),
                           };
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (_scrollId == null)
            {
                var searchResponse = _searchFunc();

                if (_progress != null)
                {
                    _progress.Report(searchResponse.Total);
                }

                if (searchResponse.Total <= 0)
                {
                    return false;
                }

                _scrollId = searchResponse.ScrollId;
            }

            if (_internalEnumerator == null || !_internalEnumerator.MoveNext())
            {
                var response = _elasticClient.Scroll<TDocument>(x => x.Scroll(_scrollTimeout).ScrollId(_scrollId));
                _scrollId = response.ScrollId;
                _internalEnumerator = response.Hits.GetEnumerator();
                return _internalEnumerator.MoveNext();
            }

            return true;
        }

        public void Dispose()
        {
            if (_internalEnumerator != null)
            {
                _internalEnumerator.Dispose();
                _internalEnumerator = null;
            }

            _scrollId = null;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}