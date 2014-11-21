using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Processors;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing
{
    public interface IPerformedOperationsFlowProcessorSettings : IMessageFlowProcessorSettings
    {
        int? TimeSafetyOffsetHours { get; }
    }
}