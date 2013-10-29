using System;

namespace DoubleGis.Erm.Platform.Model.Metadata.Common
{
    public class OrdinaryConfigElementIdentity : IConfigElementIdentity
    {
        private readonly int _id = UIDGenerator.Next;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        #region Implementation of IEquatable<ResourceEntryKey>
        bool IEquatable<IConfigElementIdentity>.Equals(IConfigElementIdentity other)
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

            if (typeof(OrdinaryConfigElementIdentity) != obj.GetType())
            {
                return false;
            }

            var other = (OrdinaryConfigElementIdentity)obj;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(OrdinaryConfigElementIdentity first, OrdinaryConfigElementIdentity second)
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

        public static bool operator !=(OrdinaryConfigElementIdentity first, OrdinaryConfigElementIdentity second)
        {
            return !(first == second);
        }
    }
}