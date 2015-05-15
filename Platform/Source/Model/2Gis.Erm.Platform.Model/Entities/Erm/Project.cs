using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Project :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeactivatableEntity,
        IStateTrackingEntity
    {
        public Project()
        {
            SalesModelRestrictions = new HashSet<SalesModelCategoryRestriction>();
        }

        public long Id { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string NameLat { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string DefaultLang { get; set; }

        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<SalesModelCategoryRestriction> SalesModelRestrictions { get; set; }

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