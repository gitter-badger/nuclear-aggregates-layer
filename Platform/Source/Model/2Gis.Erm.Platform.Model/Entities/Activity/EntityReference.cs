using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    // FIXME {s.pomadin, 21.08.2014}: ����� ��������� ����� ��� ������, �.�. EntityReference ��� ����
    public abstract class EntityReference<TEntity>
        where TEntity : IEntity
    {
        public long SourceEntityId { get; set; }
        public IEntityType TargetEntityName { get; set; }
        public long TargetEntityId { get; set; }

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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((EntityReference<TEntity>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SourceEntityId.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetEntityName.AsInt32();
                hashCode = (hashCode * 397) ^ TargetEntityId.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(EntityReference<TEntity> other)
        {
            return SourceEntityId == other.SourceEntityId
                && TargetEntityName == other.TargetEntityName
                && TargetEntityId == other.TargetEntityId;
        }
    }
}