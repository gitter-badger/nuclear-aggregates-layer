using DoubleGis.Erm.Platform.API.Core.Settings.Environments;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface IApiSettings : ISettings
    {
        EnvironmentType TargetEnvironmentType { get; }
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