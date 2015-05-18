using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed class Department :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public Department()
        {
            Children = new HashSet<Department>();
            Users = new HashSet<User>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public int? LeftBorder { get; set; }
        public int? RightBorder { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }        
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public ICollection<Department> Children { get; set; }
        public Department Parent { get; set; }
        public ICollection<User> Users { get; set; }

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