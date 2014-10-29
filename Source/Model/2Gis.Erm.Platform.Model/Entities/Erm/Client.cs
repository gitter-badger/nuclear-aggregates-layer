using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class Client :
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

        public Client()
        {
            Contacts = new HashSet<Contact>();
            Deals = new HashSet<Deal>();
            Firms = new HashSet<Firm>();
            LegalPersons = new HashSet<LegalPerson>();
        }

        public long Id { get; set; }
        public long? DgppId { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public string MainPhoneNumber { get; set; }
        public string AdditionalPhoneNumber1 { get; set; }
        public string AdditionalPhoneNumber2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public long? MainFirmId { get; set; }
        public string MainAddress { get; set; }
        public string Comment { get; set; }
        public long TerritoryId { get; set; }
        public int InformationSource { get; set; }
        public int PromisingValue { get; set; }
        public DateTime LastQualifyTime { get; set; }
        public DateTime? LastDisqualifyTime { get; set; }
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
        public bool IsAdvertisingAgency { get; set; }

        public Territory Territory { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<Deal> Deals { get; set; }
        public Firm Firm { get; set; }
        public ICollection<Firm> Firms { get; set; }
        public ICollection<LegalPerson> LegalPersons { get; set; }

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