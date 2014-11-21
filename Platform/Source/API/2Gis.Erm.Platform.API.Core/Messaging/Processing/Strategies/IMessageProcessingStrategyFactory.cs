using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies
{
    public interface IMessageProcessingStrategyFactory
    {
        IMessageProcessingStrategy Create(IMessageFlow messageFlow, IMessage message);
    }
}