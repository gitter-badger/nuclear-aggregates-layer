using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public interface IEnvironmentSettings : ISettings
    {
        EnvironmentType Type { get; }
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}