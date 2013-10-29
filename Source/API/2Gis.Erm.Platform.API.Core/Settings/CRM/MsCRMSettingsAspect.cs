using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    public sealed class MsCRMSettingsAspect : IMsCrmSettings
    {
        private readonly ConnectionStringsSettingsAspect _connectionStringsSettings;

        private readonly BoolSetting _enableReplication = ConfigFileSetting.Bool.Required("EnableReplication");
        private readonly StringSetting _crmHost = ConfigFileSetting.String.Optional("CrmHost", "");
        private bool _crmInited;
        private string _crmOrganizationName;

        public MsCRMSettingsAspect(ConnectionStringsSettingsAspect connectionStringsSettings)
        {
            _connectionStringsSettings = connectionStringsSettings;
        }
        
        public bool EnableReplication 
        {
            get { return _enableReplication.Value; }
        }

        public string CrmOrganizationName
        {
            get
            {
                if (!_crmInited)
                {
                    _crmInited = true;
                    InitCrmSettings();
                }

                return _crmOrganizationName;
            }
        }

        public string CrmHost
        {
            get { return _crmHost.Value; }
        }

        // протестировать
        public string GetCrmConnectionStringForOrganization(string organizationName)
        {
            string connectionString = _connectionStringsSettings.GetConnectionString(ConnectionStringName.CrmConnection);
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            var server = (string)connectionStringBuilder["server"];
            var uriBuilder = new UriBuilder(server) { Path = string.Concat("/", organizationName) };
            connectionStringBuilder["server"] = uriBuilder.Uri.ToString();

            return connectionStringBuilder.ConnectionString;
        }

        private void InitCrmSettings()
        {
            string connectionString = _connectionStringsSettings.GetConnectionString(ConnectionStringName.CrmConnection);
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            var server = (string)connectionStringBuilder["server"];
            var uriBuilder = new UriBuilder(server);

            _crmOrganizationName = uriBuilder.Path.Trim('/');
        }
    }
}
