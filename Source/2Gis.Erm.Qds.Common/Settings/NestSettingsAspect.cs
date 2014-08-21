﻿using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.Common.Settings;

using Elasticsearch.Net.ConnectionPool;

using Nest;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Qds.Common.Settings
{
    public sealed class NestSettingsAspect : ISettingsAspect, INestSettings
    {
        public Protocol Protocol { get; private set; }
        public string IndexPrefix { get; private set; }
        public int BatchSize { get; private set; }
        public string BatchTimeout { get; private set; }
        public IConnectionSettingsValues ConnectionSettings { get; private set; }
        
        public NestSettingsAspect(string connectionString)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Protocol = GetSettingValue(builder, "Protocol", Protocol.None);
            IndexPrefix = GetSettingValue(builder, "IndexPrefix", (string)null).ToLowerInvariant();
            BatchSize = GetSettingValue(builder, "BatchSize", 1000);
            BatchTimeout = GetSettingValue(builder, "BatchTimeout", "1m");
            ConnectionSettings = CreateConnectionSettings(builder);
        }
       
        private static T GetSettingValue<T>(DbConnectionStringBuilder builder, string key, T defaultValue)
        {
            object value;
            if (!builder.TryGetValue(key, out value))
            {
                value = defaultValue;
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

        private static ConnectionSettings CreateConnectionSettings(DbConnectionStringBuilder connectionStringBuilder)
        {
            var urisNonParsed = (string)connectionStringBuilder["Uris"];
            var uris = JsonConvert.DeserializeObject<Uri[]>(urisNonParsed);
            var connectionPool = new StaticConnectionPool(uris);

            var connectionSettings = new ConnectionSettings(connectionPool)
            // более подробные сообщения об ошибках
            .ExposeRawResponse()
            // accept-encoding: gzip, deflate
            .EnableCompressedResponses()
            // на тестовом кластере живая нода может пинговаться долго, таймаут по умолчанию не подходит
            .SetPingTimeout(2000)
            // кидать исключения вместо выставления IResponse.IsValid
            .ThrowOnElasticsearchServerExceptions()
            ;

            return connectionSettings;
        }
    }
}