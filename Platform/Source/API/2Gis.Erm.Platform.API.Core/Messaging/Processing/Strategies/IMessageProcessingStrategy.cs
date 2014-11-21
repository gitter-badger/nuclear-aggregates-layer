using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies
{
    public interface IMessageProcessingStrategy
    {
        IMessageFlow TargetFlow { get; }
        bool CanProcess(IMessage message);
        IProcessingResultMessage Process(IMessage message);
    }
}