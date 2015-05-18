using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Security
{
    public sealed class Privilege :
        IEntity,
        IEntityKey
    {
        public Privilege()
        {
            FunctionalPrivilegeDepths = new HashSet<FunctionalPrivilegeDepth>();
            UserEntities = new HashSet<UserEntity>();
            RolePrivileges = new HashSet<RolePrivilege>();
        }

        public long Id { get; set; }
        public int? EntityType { get; set; }
        public int Operation { get; set; }

        public ICollection<FunctionalPrivilegeDepth> FunctionalPrivilegeDepths { get; set; }
        public ICollection<UserEntity> UserEntities { get; set; }
        public ICollection<RolePrivilege> RolePrivileges { get; set; }

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