using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderPositionAdvertisement :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public long PositionId { get; set; }
        public long? AdvertisementId { get; set; }
        public long? FirmAddressId { get; set; }
        public long? CategoryId { get; set; }

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
        public long? ThemeId { get; set; }

        public Advertisement Advertisement { get; set; }
        public Category Category { get; set; }
        public FirmAddress FirmAddress { get; set; }
        public OrderPosition OrderPosition { get; set; }
        public Position Position { get; set; }
        public Theme Theme { get; set; }

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