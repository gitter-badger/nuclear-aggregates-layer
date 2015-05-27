using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class LocalMessage :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IEntityFile,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public DateTime EventDate { get; set; }
        public long FileId { get; set; }
        public LocalMessageStatus Status { get; set; }
        public string ProcessResult { get; set; }
        public long MessageTypeId { get; set; }
        public long? OrganizationUnitId { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long? ProcessingTime { get; set; }

        public File File { get; set; }
        public MessageType MessageType { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }

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