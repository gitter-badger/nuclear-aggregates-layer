using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Validators;

namespace DoubleGis.Erm.Platform.Core.Messaging.Processing.Validators
{
    public sealed class NullMessageValidator<TMessageFlow, TOriginalMessage> : MessageValidatorBase<TMessageFlow, TOriginalMessage>
        where TOriginalMessage : class, IMessage 
        where TMessageFlow : class, IMessageFlow, new()
    {
        protected override bool IsValid(TOriginalMessage message, out string report)
        {
            report = string.Empty;
            return true;
        }
    }
}
