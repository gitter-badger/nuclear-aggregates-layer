using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public interface IEnvironmentSettings : ISettings
    {
        EnvironmentType Type { get; }
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}