using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public interface IPerformedOperationsFinalFlowProcessorSettings : IMessageFlowProcessorSettings
    {
        bool IsRecoveryMode { get; }
        int Timeout { get; }
    }
}