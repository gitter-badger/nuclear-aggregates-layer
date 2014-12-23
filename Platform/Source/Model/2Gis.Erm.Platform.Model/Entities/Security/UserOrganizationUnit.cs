using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed class UserOrganizationUnit :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IStateTrackingEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long OrganizationUnitId { get; set; }

        public User User { get; set; }
        public OrganizationUnitDto OrganizationUnitDto { get; set; }

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