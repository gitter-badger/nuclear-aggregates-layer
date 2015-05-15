using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed class User : IEntity,
                               IEntityKey,
                               IAuditableEntity,
                               IDeletableEntity,
                               IDeactivatableEntity,
                               IStateTrackingEntity,
                               IReplicableExplicitly
    {
        public User()
        {
            UserOrganizationUnits = new HashSet<UserOrganizationUnit>();
            UserRoles = new HashSet<UserRole>();
            Children = new HashSet<User>();
            UserTerritories = new HashSet<UserTerritory>();
            UserEntities = new HashSet<UserEntity>();
            UserProfiles = new HashSet<UserProfile>();
        }

        public long Id { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long DepartmentId { get; set; }
        public long? ParentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string DisplayName { get; set; }
        public bool IsServiceUser { get; set; }

        public Department Department { get; set; }
        public ICollection<UserOrganizationUnit> UserOrganizationUnits { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<User> Children { get; set; }
        public User Parent { get; set; }
        public ICollection<UserTerritory> UserTerritories { get; set; }
        public ICollection<UserEntity> UserEntities { get; set; }
        public ICollection<UserProfile> UserProfiles { get; set; }

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