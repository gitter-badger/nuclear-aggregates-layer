using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IApiSettings : ISettings, IAPIServiceSettingsHost
    {
        AppTargetEnvironment TargetEnvironment { get; }
        string TargetEnvironmentName { get; }
        string EntryPointName { get; }

        string ApiUrl { get; }

        string ListServiceEndpointName { get; }
        string CreateOrUpdateServiceEndpointName { get; }
        string GetDomainEntityDtoServiceEndpointName { get; }
        string MetadataServiceEndpointName { get; }
        string AssignServiceEndpointName { get; }
        string GroupAssignServiceEndpointName { get; }
    }
}