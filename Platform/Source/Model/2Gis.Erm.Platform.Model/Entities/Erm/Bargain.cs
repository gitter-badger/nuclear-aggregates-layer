using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Bargain :
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

        public Bargain()
        {
            this.BargainFiles = new HashSet<BargainFile>();
            this.Orders = new HashSet<Order>();
            this.Deals = new HashSet<Deal>();
        }

        public long Id { get; set; }
        public string Number { get; set; }
        public long BargainTypeId { get; set; }
        public long CustomerLegalPersonId { get; set; }
        public long ExecutorBranchOfficeId { get; set; }
        public string Comment { get; set; }
        public DateTime SignedOn { get; set; }
        public int DgppId { get; set; }

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

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public Guid ReplicationCode { get; set; }
        public DateTime? ClosedOn { get; set; }
        public DocumentsDebt HasDocumentsDebt { get; set; }
        public string DocumentsComment { get; set; }
        public byte[] Timestamp { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public BargainKind BargainKind { get; set; }

        public ICollection<BargainFile> BargainFiles { get; set; }
        public ICollection<Order> Orders { get; set; }
        public BargainType BargainType { get; set; }
        public BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit { get; set; }
        public LegalPerson LegalPerson { get; set; }
        public ICollection<Deal> Deals { get; set; }

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