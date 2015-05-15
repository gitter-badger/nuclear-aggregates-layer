using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Account :
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

        public Account()
        {
            AccountDetails = new HashSet<AccountDetail>();
            Limits = new HashSet<Limit>();
            Locks = new HashSet<Lock>();
            Orders = new HashSet<Order>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public long? DgppId { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public long LegalPersonId { get; set; }
        public string LegalPesonSyncCode1C { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
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

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public ICollection<AccountDetail> AccountDetails { get; set; }
        public ICollection<Limit> Limits { get; set; }
        public ICollection<Lock> Locks { get; set; }
        public ICollection<Order> Orders { get; set; }
        public BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit { get; set; }
        public LegalPerson LegalPerson { get; set; }

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