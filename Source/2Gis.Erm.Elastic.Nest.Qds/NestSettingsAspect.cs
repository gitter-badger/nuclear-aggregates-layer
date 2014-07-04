using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Qds.API.Core.Settings;

using Elasticsearch.Net.ConnectionPool;

using Nest;

using Newtonsoft.Json;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public sealed class NestSettingsAspect : ISettingsAspect, INestSettings
    {
        private readonly ConnectionSettings _connectionSettings;
        private readonly string _indexPrefix;

        public NestSettingsAspect(string connectionString)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Protocol = GetSettingValue(builder, "Protocol", Protocol.None);
            _indexPrefix = GetSettingValue(builder, "IndexPrefix", (string)null).ToLowerInvariant();
            BatchSize = GetSettingValue(builder, "BatchSize", 1000);
            BatchTimeout = GetSettingValue(builder, "BatchTimeout", "1m");
            _connectionSettings = CreateConnectionSettings(builder);

            // TODO {m.pashuk, 22.04.2014}: отделить ответственность заполнения метаданными
            foreach (var indexNameMapping in IndexMappingMetadata.DocTypeToIndexNameMap)
            {
                RegisterType(indexNameMapping.Item1, indexNameMapping.Item2);
            }
        }

        public IConnectionSettingsValues ConnectionSettings
        {
            get { return _connectionSettings; }
        }

        public Protocol Protocol { get; private set; }
        public int BatchSize { get; private set; }
        public string BatchTimeout { get; private set; }
        
        public string GetIsolatedIndexName(string indexName)
        {
            return _indexPrefix + "." + indexName.ToLowerInvariant();
        }

        public void RegisterType<T>(string docIndexName, string docTypeName = null)
        {
            RegisterType(typeof(T), docIndexName, docTypeName);
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

            IConnectionPool connectionPool;
            switch (uris.Length)
            {
                case 0:
                    throw new ArgumentException("Connection string was not in correct format");
                case 1:
                    connectionPool = new SingleNodeConnectionPool(uris[0]);
                    break;
                default:
                    connectionPool = new StaticConnectionPool(uris);
                    break;
            }

            var connectionSettings = new ConnectionSettings(connectionPool);

            // более подробные сообщения об ошибках
            connectionSettings.ExposeRawResponse();

            return connectionSettings;
        }

        private void RegisterType(Type documentType, string docIndexName, string docTypeName = null)
        {
            var isolatedIndexName = string.Concat(_indexPrefix, ".", docIndexName.ToLowerInvariant());
            _connectionSettings.MapDefaultTypeIndices(x => x.Add(documentType, isolatedIndexName));

            if (docTypeName != null)
            {
                _connectionSettings.MapDefaultTypeNames(x => x.Add(documentType, docTypeName.ToLowerInvariant()));
            }
        }
    }
}