using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Contact :
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

        public long Id { get; set; }
        public long? DgppId { get; set; }
        public Guid ReplicationCode { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Salutation { get; set; }
        public Gender GenderCode { get; set; }
        public FamilyStatus FamilyStatusCode { get; set; }
        public string MainPhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string HomePhoneNumber { get; set; }
        public string Fax { get; set; }
        public string WorkEmail { get; set; }
        public string HomeEmail { get; set; }
        public string Website { get; set; }
        public string ImIdentifier { get; set; }
        public long ClientId { get; set; }
        public string JobTitle { get; set; }
        public AccountRole AccountRole { get; set; }
        public string Department { get; set; }
        public bool IsFired { get; set; }
        public DateTime? BirthDate { get; set; }
        public string WorkAddress { get; set; }
        public string HomeAddress { get; set; }
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

        public Client Client { get; set; }

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