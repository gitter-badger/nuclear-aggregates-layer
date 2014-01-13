namespace DoubleGis.Erm.Platform.API.Core.Settings.CRM
{
    /// <summary>
    /// Настройки для взаимодйствия с MS Dynamics CRM
    /// </summary>
    public interface IMsCrmSettings
    {
        bool EnableReplication { get; }

        string CrmHost { get; }
        string CrmOrganizationName { get; }
        string CrmRuntimeConnectionString { get; }
    }
}