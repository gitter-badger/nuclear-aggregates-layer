using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities
{
    [DataContract]
    public abstract class IdentityBase<TConcreteIdentity> : IIdentity where TConcreteIdentity : IdentityBase<TConcreteIdentity>, new()
    {
        private static readonly Lazy<TConcreteIdentity> SingleInstance = new Lazy<TConcreteIdentity>(() => new TConcreteIdentity());

        public static TConcreteIdentity Instance
        {
            get { return SingleInstance.Value; }
        }

        public abstract int Id { get; }
        public abstract string Description { get; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(TConcreteIdentity) != obj.GetType())
            {
                return false;
            }

            var other = (TConcreteIdentity)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format("Id={0}. Description: {1}", Id, Description);
        }

        #region Implementation of IEquatable<IOperationIdentity>

        bool IEquatable<IIdentity>.Equals(IIdentity other)
        {
            return Equals(other);
        }

        #endregion
    }
}