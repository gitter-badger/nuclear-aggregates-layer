using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.Qds.Common.Settings
{
    public sealed class NestSettingsAspect : ISettingsAspect, INestSettings
    {
        public NestSettingsAspect(IConnectionStringSettings connectionStringsSettings)
        {
            var connectionString = string.Empty;
            ConnectionStringSettings settings;
            if (connectionStringsSettings.AllConnections.TryGetValue(ConnectionStringName.ErmSearch, out settings))
            {
                connectionString = settings.ConnectionString;
            }

            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Protocol = GetSettingValue(builder, "Protocol", Protocol.Http);
            IndexPrefix = GetSettingValue(builder, "IndexPrefix", string.Empty).ToLowerInvariant();
            BatchSize = GetSettingValue(builder, "BatchSize", 1000);
            BatchTimeout = GetSettingValue(builder, "BatchTimeout", "1m");

            var endpoint = GetSettingValue(builder, "Endpoint", "localhost");
            Uris = ParseUris(endpoint, Protocol);
        }

        public Protocol Protocol { get; private set; }
        public string IndexPrefix { get; private set; }
        public int BatchSize { get; private set; }
        public string BatchTimeout { get; private set; }
        public IEnumerable<Uri> Uris { get; private set; }

        private static T GetSettingValue<T>(DbConnectionStringBuilder builder, string key, T defaultValue)
        {
            object value;
            if (!builder.TryGetValue(key, out value))
            {
                return defaultValue;
            }

            T convertedValue;
            if (typeof(Enum).IsAssignableFrom(typeof(T)))
            {
                convertedValue = (T)Enum.Parse(typeof(T), value.ToString(), true);
            }
            else
            {
                convertedValue = (T)Convert.ChangeType(value, typeof(T));
            }

            return convertedValue;
        }

        private static IEnumerable<Uri> ParseUris(string endpoint, Protocol protocol)
        {
            var uris = endpoint.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
            {
                string host;
                int port;

                var hostAndPort = x.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                switch (hostAndPort.Length)
                {
                    case 0:
                        throw new ArgumentException();
                    case 1:
                        host = hostAndPort[0].Trim();

                        switch (protocol)
                        {
                            case Protocol.Http:
                                port = 9200;
                                break;
                            case Protocol.Thrift:
                            case Protocol.ThriftCompact:
                                port = 9500;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("protocol");
                        }

                        break;
                    case 2:
                        host = hostAndPort[0].Trim();
                        port = int.Parse(hostAndPort[1]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, host, port);
                return uriBuilder.Uri;
            });

            return uris;
        }
    }
}