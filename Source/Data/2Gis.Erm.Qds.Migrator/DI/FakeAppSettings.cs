using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Qds.Migrator.DI
{
    internal sealed class FakeAppSettings : IAppSettings, IMsCrmSettings
    {
        public FakeAppSettings(IConnectionStringSettingsHost connectionStringSettingsHost)
        {
            ConnectionStrings = connectionStringSettingsHost.ConnectionStrings;
        }

        public string BasicLanguage { get; private set; }
        public string ReserveLanguage { get; private set; }
        public BusinessModel BusinessModel { get; private set; }
        public ConnectionStringsSettingsAspect ConnectionStrings { get; private set; }
        public string ReserveUserAccount { get; private set; }
        public bool EnableNotifications { get; private set; }
        public bool EnableCaching { get; private set; }
        public int SignificantDigitsNumber { get; private set; }
        public decimal MinDebtAmount { get; private set; }
        public int WarmClientDaysCount { get; private set; }
        public int OrderRequestProcessingHoursAmount { get; private set; }
        public AppTargetEnvironment TargetEnvironment { get; private set; }

        public string TargetEnvironmentName { get; private set; }
        public string EntryPointName { get; private set; }
        public bool EnableReplication { get; private set; }
        public string CrmHost { get; private set; }
        public string CrmOrganizationName { get; private set; }

        public string CrmRuntimeConnectionString { get; private set; }

        public string GetCrmConnectionStringForOrganization(string organizationName)
        {
            throw new System.NotImplementedException();
        }
    }
}