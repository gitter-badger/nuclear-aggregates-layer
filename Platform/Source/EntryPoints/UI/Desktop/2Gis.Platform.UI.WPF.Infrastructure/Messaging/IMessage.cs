using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging
{
    public interface IMessage
    {
        Guid Id { get; }
        DateTime TimestampUtc { get; }
        DateTime? ExpirationTimeUtc { get; }
        ProcessingModel ProcessingModel { get; }
    }

    public interface IMessage<TProcessingModel> : IMessage
        where TProcessingModel : class, IMessageProcessingModel, new()
    {
    }
}
