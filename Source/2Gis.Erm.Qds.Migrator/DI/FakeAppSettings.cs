using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Qds.API.Core.Settings;

namespace DoubleGis.Erm.Qds.Migrator.DI
{
    // TODO: удалить после того как SearchSettings перестанут наследоваться от CommonConfigFileAppSettings
    internal sealed class FakeAppSettings : IAppSettings, IMsCrmSettings, ISearchSettings
    {
        public FakeAppSettings()
        {
            ConnectionStrings = new ConnectionStringsSettingsAspect();

            var connectionString = ConnectionStrings.GetConnectionString(ConnectionStringName.ErmSearch);
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            Host = (string)connectionStringBuilder["Host"];
            IndexPrefix = (string)connectionStringBuilder["IndexPrefix"];
            Protocol = (Protocol)Enum.Parse(typeof(Protocol), (string)connectionStringBuilder["Protocol"], true);
            HttpPort = Convert.ToInt32(connectionStringBuilder["HttpPort"]);
            ThriftPort = Convert.ToInt32(connectionStringBuilder["ThriftPort"]);
            BatchSize = Convert.ToInt32(connectionStringBuilder["BatchSize"]);
        }

        public string Host { get; set; }
        public string IndexPrefix { get; private set; }
        public Protocol Protocol { get; set; }
        public int HttpPort { get; set; }
        public int ThriftPort { get; set; }
        public int BatchSize { get; set; }

        public string ReserveUserAccount { get; set; }
        public bool EnableNotifications { get; set; }
        public bool EnableCaching { get; set; }
        public int SignificantDigitsNumber { get; set; }
        public decimal MinDebtAmount { get; set; }
        public int WarmClientDaysCount { get; set; }
        public int OrderRequestProcessingHoursAmount { get; set; }
        public AppTargetEnvironment TargetEnvironment { get; set; }
        public string TargetEnvironmentName { get; set; }

        public string EntryPointName { get; set; }
        public string BasicLanguage { get; set; }
        public string ReserveLanguage { get; set; }
        public BusinessModel BusinessModel { get; set; }
        public ConnectionStringsSettingsAspect ConnectionStrings { get; set; }

        public bool EnableReplication { get; set; }
        public string CrmHost { get; set; }
        public string CrmOrganizationName { get; set; }
        public string CrmRuntimeConnectionString { get; set; }
    }
}