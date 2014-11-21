using System;

using Elasticsearch.Net;
using Elasticsearch.Net.Connection;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    class StubElasticsearchResponse : IElasticsearchResponse
    {
        public bool Success { get; set; }
        public bool SuccessOrKnownError { get; set; }
        public IConnectionConfigurationValues Settings { get; set; }
        public Exception OriginalException { get; set; }
        public string RequestMethod { get; set; }
        public string RequestUrl { get; set; }
        public byte[] Request { get; set; }
        public int? HttpStatusCode { get; set; }
        public int NumberOfRetries { get; set; }
        public byte[] ResponseRaw { get; set; }
        public string ToStringResult { get; set; }
        public override string ToString()
        {
            return ToStringResult;
        }

        public CallMetrics Metrics
        {
            get { throw new NotImplementedException(); }
        }
    }
}