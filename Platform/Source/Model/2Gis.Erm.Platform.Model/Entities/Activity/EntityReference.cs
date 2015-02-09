using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    // FIXME {s.pomadin, 21.08.2014}: Ќужно придумать новое им€ классу, т.к. EntityReference уже есть
    public abstract class EntityReference<TEntity>
        where TEntity : IEntity
    {
        public long SourceEntityId { get; set; }
        public int TargetEntityTypeId { get; set; }
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
                hashCode = (hashCode * 397) ^ TargetEntityTypeId;
                hashCode = (hashCode * 397) ^ TargetEntityId.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(EntityReference<TEntity> other)
        {
            return SourceEntityId == other.SourceEntityId
                && TargetEntityTypeId == other.TargetEntityTypeId
                && TargetEntityId == other.TargetEntityId;
        }
    }
}