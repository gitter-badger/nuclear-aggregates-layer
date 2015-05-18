using System;
using System.Collections.Generic;

using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderProcessingRequest :
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

        public OrderProcessingRequest()
        {
            OrderProcessingRequestMessages = new HashSet<OrderProcessingRequestMessage>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Title { get; set; }
        public OrderProcessingRequestType RequestType { get; set; }
        public DateTime DueDate { get; set; }
        public long? BaseOrderId { get; set; }
        public long? RenewedOrderId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public long FirmId { get; set; }
        public long LegalPersonProfileId { get; set; }
        public long LegalPersonId { get; set; }
        public string Description { get; set; }
        public OrderProcessingRequestState State { get; set; }
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
        public short ReleaseCountPlan { get; set; }

        public LegalPerson LegalPerson { get; set; }
        public Order BaseOrder { get; set; }
        public LegalPersonProfile LegalPersonProfile { get; set; }
        public Firm Firm { get; set; }
        public Order RenewedOrder { get; set; }
        public OrganizationUnit SourceOrganizationUnit { get; set; }
        public ICollection<OrderProcessingRequestMessage> OrderProcessingRequestMessages { get; set; }

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