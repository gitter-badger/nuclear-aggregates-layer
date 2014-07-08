using System;

namespace DoubleGis.Erm.Platform.API.Core.Messaging
{
    public interface IMessage : IEquatable<IMessage>
    {
        Guid Id { get; }
    }
}
