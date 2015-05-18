using System;

using DoubleGis.Erm.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class OrderProcessingRequestMessage :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public long OrderRequestId { get; set; }
        public RequestMessageType MessageType { get; set; }
        public int MessageTemplateCode { get; set; }
        public string MessageParameters { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public Guid GroupId { get; set; }

        public OrderProcessingRequest OrderProcessingRequest { get; set; }

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