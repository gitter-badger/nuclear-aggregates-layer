using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Position :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public Position()
        {
            MasterPositions = new HashSet<PositionChildren>();
            ChildPositions = new HashSet<PositionChildren>();
            AssociatedPositions = new HashSet<AssociatedPosition>();
            DeniedPositions = new HashSet<DeniedPosition>();
            PricePositions = new HashSet<PricePosition>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            ReleasesWithdrawalsPositions = new HashSet<ReleasesWithdrawalsPosition>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public bool IsComposite { get; set; }
        public PositionCalculationMethod CalculationMethodEnum { get; set; }
        public PositionBindingObjectType BindingObjectTypeEnum { get; set; }
        public SalesModel SalesModel { get; set; }
        public PositionsGroup PositionsGroup { get; set; }
        public long PlatformId { get; set; }
        public long CategoryId { get; set; }
        public long? AdvertisementTemplateId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long? DgppId { get; set; }
        public int ExportCode { get; set; }
        public bool IsControlledByAmount { get; set; }
        public bool RestrictChildPositionPlatforms { get; set; }
        public int? SortingIndex { get; set; }

        public PositionCategory PositionCategory { get; set; }
        public ICollection<PositionChildren> MasterPositions { get; set; }
        public ICollection<PositionChildren> ChildPositions { get; set; }
        public Platform Platform { get; set; }
        public AdvertisementTemplate AdvertisementTemplate { get; set; }
        public ICollection<AssociatedPosition> AssociatedPositions { get; set; }
        public ICollection<DeniedPosition> DeniedPositions { get; set; }
        public ICollection<PricePosition> PricePositions { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public ICollection<ReleasesWithdrawalsPosition> ReleasesWithdrawalsPositions { get; set; }

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