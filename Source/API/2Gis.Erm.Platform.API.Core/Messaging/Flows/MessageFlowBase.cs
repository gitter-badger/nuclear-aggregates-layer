using System;

namespace DoubleGis.Erm.Platform.API.Core.Messaging.Flows
{
    public abstract class MessageFlowBase<TConcreteMessageFlow> : IMessageFlow 
        where TConcreteMessageFlow : MessageFlowBase<TConcreteMessageFlow>, new()
    {
        private static readonly Lazy<TConcreteMessageFlow> SingleInstance = new Lazy<TConcreteMessageFlow>(() => new TConcreteMessageFlow());

        public static TConcreteMessageFlow Instance
        {
            get { return SingleInstance.Value; }
        }

        public abstract Guid Id { get; }
        public abstract string Description { get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(TConcreteMessageFlow) != obj.GetType())
            {
                return false;
            }

            var other = (TConcreteMessageFlow)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}. Id={1}", GetType().Name, Id);
        }

        #region Implementation of IEquatable<IOperationIdentity>

        bool IEquatable<IMessageFlow>.Equals(IMessageFlow other)
        {
            return Equals(other);
        }

        #endregion
    }
}