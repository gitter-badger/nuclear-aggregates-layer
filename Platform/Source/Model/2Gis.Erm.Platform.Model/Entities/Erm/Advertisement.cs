using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Advertisement :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public Advertisement()
        {
            AdvertisementElements = new HashSet<AdvertisementElement>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
        }

        public long Id { get; set; }
        public long? FirmId { get; set; }
        public long AdvertisementTemplateId { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
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

        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsSelectedToWhiteList { get; set; }
        public long? DgppId { get; set; }

        public AdvertisementTemplate AdvertisementTemplate { get; set; }
        public ICollection<AdvertisementElement> AdvertisementElements { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public Firm Firm { get; set; }

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