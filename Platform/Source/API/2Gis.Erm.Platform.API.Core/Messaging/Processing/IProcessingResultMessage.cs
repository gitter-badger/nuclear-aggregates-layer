using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Processing
{
    public interface IProcessingResultMessage : IMessage
    {
        IMessageFlow TargetFlow { get; }
    }
}