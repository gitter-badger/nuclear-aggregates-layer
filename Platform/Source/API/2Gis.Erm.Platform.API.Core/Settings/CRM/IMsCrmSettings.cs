using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    /// <summary>
    /// Настройки для взаимодйствия с MS Dynamics CRM
    /// </summary>
    public interface IMsCrmSettings : ISettings
    {
        MsCrmIntegrationMode IntegrationMode { get; }

        string CrmOrganizationName { get; }
        string CrmRuntimeConnectionString { get; }
    }
}