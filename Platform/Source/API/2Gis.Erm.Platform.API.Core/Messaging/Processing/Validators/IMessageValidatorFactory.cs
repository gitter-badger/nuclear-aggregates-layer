using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators
{
    public interface IMessageValidatorFactory
    {
        IMessageValidator Create<TMessageFlow>(IMessage message) where TMessageFlow : class, IMessageFlow, new();
    }
}