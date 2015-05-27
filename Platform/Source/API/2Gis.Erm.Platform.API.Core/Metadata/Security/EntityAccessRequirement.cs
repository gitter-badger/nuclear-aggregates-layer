using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Metadata.Security
{
    public sealed class EntityAccessRequirement : IAccessRequirement
    {
        private readonly EntityAccessTypes _requirement;
        private readonly IEntityType _entityName;

        public EntityAccessRequirement(EntityAccessTypes requirement, IEntityType entityName)
        {
            _requirement = requirement;
            _entityName = entityName;
        }

        public EntityAccessTypes EntityAccessType
        {
            get { return _requirement; }
        }

        public IEntityType EntityName
        {
            get { return _entityName; }
        }

        public bool Equals(IAccessRequirement obj)
        {
            var other = obj as EntityAccessRequirement;
            return other != null && Equals(other);
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityAccessRequirement;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)EntityAccessType * 397) ^ EntityName.Id;
            }
        }

        private bool Equals(EntityAccessRequirement other)
        {
            return EntityAccessType == other.EntityAccessType && EntityName == other.EntityName;
        }
    }
}
