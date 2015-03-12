using NuClear.Settings;
using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public sealed class EnvironmentsAspect : ISettingsAspect, IEnvironmentSettings
    {
        private readonly EnumSetting<EnvironmentType> _targetEnvironment = ConfigFileSetting.Enum.Required<EnvironmentType>("TargetEnvironment");
        private readonly StringSetting _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName");
        private readonly StringSetting _entryPointName = ConfigFileSetting.String.Required("EntryPointName");

        public EnvironmentType Type
        {
            get { return _targetEnvironment.Value; }
        }

        public string EntryPointName
        {
            get { return _entryPointName.Value; }
        }

        public string EnvironmentName
        {
            get { return _targetEnvironmentName.Value; }
        }
    }
}
