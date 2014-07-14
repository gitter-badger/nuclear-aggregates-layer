using System;
using System.Net;

using Elasticsearch.Net.Connection;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public sealed class WindowsAuthHttpConnection : HttpConnection
    {
        public WindowsAuthHttpConnection(IConnectionConfigurationValues settings)
            : base(settings)
        {
        }

        protected override HttpWebRequest CreateHttpWebRequest(Uri uri, string method, byte[] data, IRequestConnectionConfiguration requestSpecificConfig)
        {
            var httpWebRequest = CreateWebRequest(uri, method, data, requestSpecificConfig);

            // using windows credentials
            httpWebRequest.UseDefaultCredentials = true;
            httpWebRequest.PreAuthenticate = true;

            // allow traffic compression
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return httpWebRequest;
        }
    }
}
