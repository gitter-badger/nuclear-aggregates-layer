using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class BranchOffice :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public BranchOffice()
        {
            Bargains = new HashSet<Bargain>();
            BranchOfficeOrganizationUnits = new HashSet<BranchOfficeOrganizationUnit>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public long? DgppId { get; set; }
        public string Name { get; set; }
        public string LegalAddress { get; set; }
        public string Inn { get; set; }
        public long? BargainTypeId { get; set; }
        public long? ContributionTypeId { get; set; }
        public string Annotation { get; set; }
        public string UsnNotificationText { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string Ic { get; set; }

        public ICollection<Bargain> Bargains { get; set; }
        public BargainType BargainType { get; set; }
        public ContributionType ContributionType { get; set; }
        public ICollection<BranchOfficeOrganizationUnit> BranchOfficeOrganizationUnits { get; set; }

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