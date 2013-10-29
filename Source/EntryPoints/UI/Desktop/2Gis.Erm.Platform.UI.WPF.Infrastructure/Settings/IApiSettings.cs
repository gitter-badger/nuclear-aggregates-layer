using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IApiSettings : ISettings
    {
        string ListServiceEndpointName { get; }
        string CreateOrUpdateServiceEndpointName { get; }
        string GetDomainEntityDtoServiceEndpointName { get; }
        string MetadataServiceEndpointName { get; }
        string AssignServiceEndpointName { get; }
        string GroupAssignServiceEndpointName { get; }
    }
}