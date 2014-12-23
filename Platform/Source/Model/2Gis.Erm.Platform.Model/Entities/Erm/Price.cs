using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Price :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public Price()
        {
            LockDetails = new HashSet<LockDetail>();
            DeniedPositions = new HashSet<DeniedPosition>();
            PricePositions = new HashSet<PricePosition>();
        }

        public long Id { get; set; }
        public long? DgppId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime BeginDate { get; set; }
        public bool IsPublished { get; set; }
        public long OrganizationUnitId { get; set; }
        public long CurrencyId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public Currency Currency { get; set; }
        public ICollection<LockDetail> LockDetails { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<DeniedPosition> DeniedPositions { get; set; }
        public ICollection<PricePosition> PricePositions { get; set; }

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