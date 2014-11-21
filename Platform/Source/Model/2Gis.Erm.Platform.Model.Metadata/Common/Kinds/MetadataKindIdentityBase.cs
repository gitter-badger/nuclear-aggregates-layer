using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds
{
    [DataContract]
    public abstract class MetadataKindIdentityBase<TConcreteIdentity> : IMetadataKindIdentity 
        where TConcreteIdentity : MetadataKindIdentityBase<TConcreteIdentity>, new()
    {
        private static readonly Lazy<TConcreteIdentity> SingleInstance = new Lazy<TConcreteIdentity>(() => new TConcreteIdentity());

        public static TConcreteIdentity Instance
        {
            get { return SingleInstance.Value; }
        }

        public abstract Uri Id { get; }
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
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Id={0}. Description: {1}", Id, Description);
        }

        #region Implementation of IEquatable<IMetadataKindIdentity>

        bool IEquatable<IMetadataKindIdentity>.Equals(IMetadataKindIdentity other)
        {
            return Equals(other);
        }

        #endregion
    }
}