using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Category :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public Category()
        {
            ChildCategories = new HashSet<Category>();
            CategoryFirmAddresses = new HashSet<CategoryFirmAddress>();
            CategoryOrganizationUnits = new HashSet<CategoryOrganizationUnit>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            ThemeCategories = new HashSet<ThemeCategory>();
            SalesModelRestrictions = new HashSet<SalesModelCategoryRestriction>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Comment { get; set; }

        public ICollection<Category> ChildCategories { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<CategoryFirmAddress> CategoryFirmAddresses { get; set; }
        public ICollection<CategoryOrganizationUnit> CategoryOrganizationUnits { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public ICollection<ThemeCategory> ThemeCategories { get; set; }
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