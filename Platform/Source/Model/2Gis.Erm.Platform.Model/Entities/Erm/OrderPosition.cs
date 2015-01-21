using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderPosition :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public OrderPosition()
        {
            ReleasesWithdrawals = new HashSet<ReleaseWithdrawal>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
        }

        public long Id { get; set; }
        public long OrderId { get; set; }
        public long? DgppId { get; set; }
        public Guid ReplicationCode { get; set; }
        public long PricePositionId { get; set; }
        public int Amount { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PricePerUnitWithVat { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public decimal PayablePrice { get; set; }
        public decimal PayablePlanWoVat { get; set; }
        public decimal PayablePlan { get; set; }
        public int ShipmentPlan { get; set; }
        public string Comment { get; set; }
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
        public decimal CategoryRate { get; set; }

        public ICollection<ReleaseWithdrawal> ReleasesWithdrawals { get; set; }
        public PricePosition PricePosition { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public Order Order { get; set; }

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