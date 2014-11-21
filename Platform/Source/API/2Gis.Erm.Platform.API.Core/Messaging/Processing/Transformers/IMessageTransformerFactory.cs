using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Transformers
{
    public interface IMessageTransformerFactory
    {
        IMessageTransformer Create(IMessageFlow messageFlow,  IMessage message);
    }
}