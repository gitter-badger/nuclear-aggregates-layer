using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class BargainFile :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IEntityFile,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public long Id { get; set; }
        public long BargainId { get; set; }
        public long FileId { get; set; }
        public string Comment { get; set; }
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
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public BargainFileKind FileKind { get; set; }

        public Bargain Bargain { get; set; }
        public File File { get; set; }

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