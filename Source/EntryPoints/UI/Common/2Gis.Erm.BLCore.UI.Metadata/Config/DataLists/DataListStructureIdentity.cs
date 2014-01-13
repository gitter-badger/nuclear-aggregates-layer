using System;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public class DataListStructureIdentity : IConfigElementIdentity
    {
        private readonly EntityName _entityName;
        private readonly int _id = UIDGenerator.Next;

        public DataListStructureIdentity(EntityName entityName)
        {
            _entityName = entityName;
        }

        public int Id
        {
            get { return _id; }
        }

        public EntityName EntityName
        {
            get { return _entityName; }
        }

        #region Equality members
        bool IEquatable<IConfigElementIdentity>.Equals(IConfigElementIdentity other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (typeof(DataListStructureIdentity) != obj.GetType())
            {
                return false;
            }

            var other = (DataListStructureIdentity)obj;

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return _id == other._id || _entityName == other._entityName;
        }

        public override int GetHashCode()
        {
            return _id;
        }

        public static bool operator ==(DataListStructureIdentity first, DataListStructureIdentity second)
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

        public static bool operator !=(DataListStructureIdentity first, DataListStructureIdentity second)
        {
            return !(first == second);
        }

        #endregion
    }
}