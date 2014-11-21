using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete
{
    public sealed class MetadataElementIdentity : IMetadataElementIdentity
    {
        private readonly Uri _id;

        public MetadataElementIdentity(Uri elementId)
        {
            _id = elementId;
        }

        public Uri Id
        {
            get
            {
                return _id;
            }
        }

        public static bool operator ==(MetadataElementIdentity first, MetadataElementIdentity second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(MetadataElementIdentity first, MetadataElementIdentity second)
        {
            return !(first == second);
        }

        #region Implementation of IEquatable<IMetadataElementIdentity>
        bool IEquatable<IMetadataElementIdentity>.Equals(IMetadataElementIdentity other)
        {
            return Equals(other);
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(MetadataElementIdentity) != obj.GetType())
            {
                return false;
            }

            var other = (MetadataElementIdentity)obj;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}