using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Qds.API.Core.Settings
{
    public sealed class SearchSettingsAspect : ISettingsAspect, ISearchSettings
    {
        public SearchSettingsAspect(IConnectionStringSettings connectionStringSettings)
        {
            var connectionStringBuilder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionStringSettings.GetConnectionString(ConnectionStringName.ErmSearch)
                };

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