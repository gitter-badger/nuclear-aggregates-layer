using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public class GridStructureIdentity : IGridStructureIdentity
    {
        private readonly EntityName _entityName;
        private readonly int _id = UIDGenerator.Next;

        public GridStructureIdentity(EntityName entityName)
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

        #region IEquatable<IConfigElementIdentity>

        public bool Equals(IConfigElementIdentity other)
        {
            return Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((GridStructureIdentity)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)_entityName * 397) ^ _id;
            }
        }

        public static bool operator ==(GridStructureIdentity left, GridStructureIdentity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GridStructureIdentity left, GridStructureIdentity right)
        {
            return !Equals(left, right);
        }

        protected bool Equals(GridStructureIdentity other)
        {
            return _entityName == other._entityName && _id == other._id;
        }

        #endregion
    }
}