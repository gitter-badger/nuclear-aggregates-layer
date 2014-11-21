using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators
{
    public abstract class MessageValidatorBase<TMessageFlow, TMessage> : IMessageValidator
        where TMessageFlow : class, IMessageFlow, new()
        where TMessage : class, IMessage
    {
        protected readonly TMessageFlow MessageFlow = new TMessageFlow();

        bool IMessageValidator.CanValidate(IMessage message)
        {
            var concreteMessage = message as TMessage;
            if (concreteMessage == null)
            {
                return false;
            }

            return CanValidate(concreteMessage);
        }

        bool IMessageValidator.IsValid(IMessage message, out string report)
        {
            return IsValid((TMessage)message, out report);
        }

        protected virtual bool CanValidate(TMessage message)
        {
            return true;
        }

        protected abstract bool IsValid(TMessage message, out string report);
    }
}