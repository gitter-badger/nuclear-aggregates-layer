using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public sealed class SearchSettings : CommonConfigFileAppSettings, ISearchSettings
    {
        public SearchSettings()
        {
            var connectionString = ConnectionStrings.GetConnectionString(ConnectionStringName.ErmSearch);
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Host = (string)connectionStringBuilder["Host"];
            IndexPrefix = (string)connectionStringBuilder["IndexPrefix"];
            Protocol = (Protocol)Enum.Parse(typeof(Protocol), (string)connectionStringBuilder["Protocol"], true);
            HttpPort = Convert.ToInt32(connectionStringBuilder["HttpPort"]);
            ThriftPort = Convert.ToInt32(connectionStringBuilder["ThriftPort"]);
            BatchSize = Convert.ToInt32(connectionStringBuilder["BatchSize"]);
        }

        public string Host { get; private set; }
        public string IndexPrefix { get; private set; }
        public Protocol Protocol { get; private set; }
        public int HttpPort { get; private set; }
        public int ThriftPort { get; private set; }
        public int BatchSize { get; private set; }
    }
}