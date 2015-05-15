using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class PricePosition :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public PricePosition()
        {
            AssociatedPositionsGroups = new HashSet<AssociatedPositionsGroup>();
            OrderPositions = new HashSet<OrderPosition>();
        }

        public long Id { get; set; }
        public long? DgppId { get; set; }
        public long PriceId { get; set; }
        public long PositionId { get; set; }
        public decimal Cost { get; set; }
        public int? Amount { get; set; }
        public PricePositionAmountSpecificationMode AmountSpecificationMode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

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
        public int? MinAdvertisementAmount { get; set; }
        public int? MaxAdvertisementAmount { get; set; }
        public PricePositionRateType RateType { get; set; }

        public ICollection<AssociatedPositionsGroup> AssociatedPositionsGroups { get; set; }
        public ICollection<OrderPosition> OrderPositions { get; set; }
        public Position Position { get; set; }
        public Price Price { get; set; }

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