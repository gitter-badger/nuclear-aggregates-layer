using System;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    /// <summary>
    /// Настройки для взаимодйствия с MS Dynamics CRM
    /// </summary>
    public interface IMsCrmSettings : ISettings
    {
        [Obsolete("Use IntegrationMode instead")]
        bool EnableReplication { get; }
        MsCrmIntegrationMode IntegrationMode { get; }

        string CrmOrganizationName { get; }
        string CrmRuntimeConnectionString { get; }
    }
}