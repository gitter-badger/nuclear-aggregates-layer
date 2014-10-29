using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    // FIXME {a.rechkalov, 17.09.2014}: Этот класс и функционал, связанный с ним не нужны больше
    public sealed class RegionalAdvertisingSharing :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IStateTrackingEntity
    {
        public RegionalAdvertisingSharing()
        {
            OrdersRegionalAdvertisingSharings = new HashSet<OrdersRegionalAdvertisingSharing>();
        }

        public long Id { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long SourceBranchOfficeOrganizationUnitId { get; set; }
        public long DestBranchOfficeOrganizationUnitId { get; set; }
        public decimal TotalAmount { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public BranchOfficeOrganizationUnit SourceBranchOfficeOrganizationUnit { get; set; }
        public BranchOfficeOrganizationUnit DestBranchOfficeOrganizationUnit { get; set; }
        public ICollection<OrdersRegionalAdvertisingSharing> OrdersRegionalAdvertisingSharings { get; set; }
        public OrganizationUnit SourceOrganizationUnit { get; set; }
        public OrganizationUnit DestOrganizationUnit { get; set; }

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