using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class OperationLoggingSettingsAspect : ISettingsAspect, IOperationLoggingSettings
    {
        private readonly EnumSetting<LoggingTargets> _operationLoggingTargets =
            ConfigFileSetting.Enum.Optional("OperationLoggingTargets", LoggingTargets.DB);

        public LoggingTargets OperationLoggingTargets
        {
            get { return _operationLoggingTargets.Value; }
        }
    }
}