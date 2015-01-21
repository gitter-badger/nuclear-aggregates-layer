using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Platform :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IStateTrackingEntity
    {
        public Platform()
        {
            Positions = new HashSet<Position>();
            Orders = new HashSet<Order>();
            ReleasesWithdrawalsPositions = new HashSet<ReleasesWithdrawalsPosition>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public PositionPlatformPlacementPeriod PlacementPeriodEnum { get; set; }
        public PositionPlatformMinPlacementPeriod MinPlacementPeriodEnum { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long DgppId { get; set; }
        public bool IsSupportedByExport { get; set; }

        public ICollection<Position> Positions { get; set; }
        public ICollection<Order> Orders { get; set; }
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