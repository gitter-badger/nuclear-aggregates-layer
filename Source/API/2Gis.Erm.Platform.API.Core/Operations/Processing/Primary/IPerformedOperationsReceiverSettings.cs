using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public interface IPerformedOperationsReceiverSettings : IMessageReceiverSettings
    {
        int TimeSafetyOffsetHours { get; }
    }
}