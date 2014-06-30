using System;
using System.Net;

using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.Common;

using Elasticsearch.Net.Connection;
using Elasticsearch.Net.Connection.Thrift;

using Microsoft.Practices.Unity;

using Nest;

namespace DoubleGis.Erm.BLQuerying.DI
{
    public sealed class UnityElasticApiFactory
    {
        private readonly IUnityContainer _container;
        private readonly INestSettings _nestSettings;
        private readonly Func<IConnection> _createElasticConnection;

        public UnityElasticApiFactory(IUnityContainer container, INestSettings nestSettings)
        {
            _container = container;
            _nestSettings = nestSettings;

            switch (_nestSettings.Protocol)
            {
                case Protocol.Http:
                    {
                        _createElasticConnection = () => new WindowsAuthHttpConnection(_nestSettings.ConnectionSettings);
                    }

                    break;
                case Protocol.Thrift:
                    {
                        _createElasticConnection = () => new ThriftConnection(_nestSettings.ConnectionSettings);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // FIXME {all, 22.04.2014}: Вызов этого метода дублируется в данный момент, а вероятно должен остаться только в QueryingBootstrapper
        public ElasticApi CreateElasticApi(Func<LifetimeManager> lifetime)
        {
            var connection = _createElasticConnection();
            // register disposable connection (in Thrift case)
            _container.RegisterInstance(connection, lifetime());

            var responseHandler = _container.Resolve<IElasticResponseHandler>();
            var elasticClient = new ElasticClient(_nestSettings.ConnectionSettings, connection);
            var elasticApi = new ElasticApi(elasticClient, _nestSettings, responseHandler);
            return elasticApi;
        }

        private sealed class WindowsAuthHttpConnection : HttpConnection
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
}