using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class ReleaseWithdrawal :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IStateTrackingEntity
    {
        public ReleaseWithdrawal()
        {
            ReleasesWithdrawalsPositions = new HashSet<ReleasesWithdrawalsPosition>();
        }

        public long Id { get; set; }
        public long OrderPositionId { get; set; }
        public int ReleaseNumber { get; set; }
        public DateTime ReleaseBeginDate { get; set; }
        public DateTime ReleaseEndDate { get; set; }
        public decimal AmountToWithdraw { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal Vat { get; set; }
        public byte[] Timestamp { get; set; }

        public OrderPosition OrderPosition { get; set; }
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