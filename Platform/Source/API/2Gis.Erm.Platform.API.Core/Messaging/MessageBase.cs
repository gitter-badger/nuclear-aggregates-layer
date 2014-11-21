using System;

namespace DoubleGis.Erm.Platform.API.Core.Messaging
{
    public abstract class MessageBase : IMessage
    {
        public abstract Guid Id { get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var other = (IMessage)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #region Implementation of IEquatable<IOperationIdentity>

        bool IEquatable<IMessage>.Equals(IMessage other)
        {
            return Equals(other);
        }

        #endregion
    }
}