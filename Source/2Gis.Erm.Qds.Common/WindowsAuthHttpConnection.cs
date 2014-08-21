using System;
using System.Net;

using Elasticsearch.Net.Connection;
using Elasticsearch.Net.Connection.Configuration;

namespace DoubleGis.Erm.Qds.Common
{
    public sealed class WindowsAuthHttpConnection : HttpConnection
    {
        public WindowsAuthHttpConnection(IConnectionConfigurationValues settings)
            : base(settings)
        {
        }

        protected override HttpWebRequest CreateHttpWebRequest(Uri uri, string method, byte[] data, IRequestConfiguration requestSpecificConfig)
        {
            var httpWebRequest = CreateWebRequest(uri, method, data, requestSpecificConfig);

            // using windows credentials
            httpWebRequest.UseDefaultCredentials = true;
            httpWebRequest.PreAuthenticate = true;

            return httpWebRequest;
        }
    }
}