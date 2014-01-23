using System;

using DoubleGis.Erm.Qds.API.Core.Settings;

using Nest;
using Nest.Thrift;

namespace DoubleGis.Erm.Qds.Common.ElasticClient
{
    public sealed class ElasticClientFactory : IElasticClientFactory
    {
        private readonly ISearchSettings _searchSettings;
        private readonly IElasticConnectionSettingsFactory _connectionSettingsFactory;

        public ElasticClientFactory(ISearchSettings searchSettings, IElasticConnectionSettingsFactory connectionSettingsFactory)
        {
            _searchSettings = searchSettings;
            _connectionSettingsFactory = connectionSettingsFactory;
        }

        public void UsingElasticClient(Action<IElasticClient> action)
        {
            Func<IElasticClient, bool> func = x =>
            {
                action(x);
                return true;
            };

            UsingElasticClient(func);
        }

        public TResult UsingElasticClient<TResult>(Func<IElasticClient, TResult> func)
        {
            var uriBuilder = new UriBuilder(_searchSettings.Host);

            IConnectionSettings connectionSettings;
            IConnection connection;
            IDisposable resource;
            switch (_searchSettings.Protocol)
            {
                case Protocol.Http:
                    {
                        uriBuilder.Port = _searchSettings.HttpPort;
                        connectionSettings = _connectionSettingsFactory.CreateConnectionSettings(uriBuilder.Uri);
                        connection = new Connection(connectionSettings);
                        resource = null;
                    }

                    break;
                case Protocol.Thrift:
                    {
                        uriBuilder.Port = _searchSettings.ThriftPort;
                        connectionSettings = _connectionSettingsFactory.CreateConnectionSettings(uriBuilder.Uri);
                        var thriftConnection = new ThriftConnection(connectionSettings);
                        connection = thriftConnection;
                        resource = thriftConnection;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            using (resource)
            {
                var elasticClient = new Nest.ElasticClient(connectionSettings, connection);

                return func(elasticClient);
            }
        }
    }
}