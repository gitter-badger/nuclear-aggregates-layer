using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationLoggingSettings : ISettings
    {
        LoggingTargets OperationLoggingTargets { get; }
    }
}