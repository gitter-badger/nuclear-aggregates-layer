using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationLoggingSettings : ISettings
    {
        LoggingTargets OperationLoggingTargets { get; }
    }
}