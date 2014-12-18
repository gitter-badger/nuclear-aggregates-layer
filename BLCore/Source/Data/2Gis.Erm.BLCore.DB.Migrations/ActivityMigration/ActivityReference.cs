using System;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration
{
    using ErmEntityName = Metadata.Erm.EntityName;

    public sealed class ActivityReference : IEquatable<ActivityReference>
    {
        public ActivityReference(ErmEntityName entityName, long entityId)
        {
            EntityName = entityName;
            EntityId = entityId;
        }

        public ErmEntityName EntityName { get; private set; }
        public long EntityId { get; private set; }
        
        public bool Equals(ActivityReference other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return EntityName == other.EntityName && EntityId == other.EntityId;
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
            return obj is ActivityReference && Equals((ActivityReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)EntityName * 397) ^ EntityId.GetHashCode();
            }
        }
    }
}