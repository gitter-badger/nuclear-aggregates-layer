using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class DictionaryEntityInstance : IEntity,
                                                   IEntityKey,
                                                   ICuratedEntity,
                                                   IAuditableEntity,
                                                   IDeletableEntity,
                                                   IDeactivatableEntity,
                                                   IStateTrackingEntity,
                                                   IDynamicEntityInstance
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public DictionaryEntityInstance()
        {
            DictionaryEntityPropertyInstances = new HashSet<DictionaryEntityPropertyInstance>();
        }

        public long Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public long OwnerCode
        {
            get { return _ownerCode; }

            set
            {
                _oldOwnerCode = _ownerCode;
                _ownerCode = value;
            }
        }

        long? ICuratedEntity.OldOwnerCode
        {
            get { return _oldOwnerCode; }
        }

        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long? EntityId { get; set; }

        public ICollection<DictionaryEntityPropertyInstance> DictionaryEntityPropertyInstances { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}