using System;
using System.Data.Common;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    public sealed class MsCRMSettingsAspect : ISettingsAspect, IMsCrmSettings
    {
        private readonly ConnectionStringsSettingsAspect _connectionStringsSettings;

        private readonly EnumSetting<MsCrmIntegrationMode> _integrationMode = ConfigFileSetting.Enum.Required<MsCrmIntegrationMode>("MsCrmIntegrationMode");

        private readonly Lazy<string> _crmOrganizationName;

        public MsCRMSettingsAspect(ConnectionStringsSettingsAspect connectionStringsSettings)
        {
            _connectionStringsSettings = connectionStringsSettings;
            _crmOrganizationName = new Lazy<string>(ExtractOrganizationName);
        }

        public MsCrmIntegrationMode IntegrationMode
        {
            get { return _integrationMode.Value; }
        }

        public string CrmOrganizationName
        {
            get { return _crmOrganizationName.Value; }
        }

        public string CrmRuntimeConnectionString
        {
            get { return _connectionStringsSettings.GetConnectionString(ConnectionStringName.CrmConnection); }
        }

        private string ExtractOrganizationName()
        {
            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = CrmRuntimeConnectionString };
            var server = (string)connectionStringBuilder["server"];
            var uriBuilder = new UriBuilder(server);
            return uriBuilder.Path.Trim('/');
        }
    }
}