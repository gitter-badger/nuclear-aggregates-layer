using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Theme :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IEntityFile,
        IStateTrackingEntity
    {
        public Theme()
        {
            ThemeCategories = new HashSet<ThemeCategory>();
            ThemeOrganizationUnits = new HashSet<ThemeOrganizationUnit>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
        }

        public long Id { get; set; }
        public long ThemeTemplateId { get; set; }
        public long FileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BeginDistribution { get; set; }
        public DateTime EndDistribution { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public ICollection<ThemeCategory> ThemeCategories { get; set; }
        public ICollection<ThemeOrganizationUnit> ThemeOrganizationUnits { get; set; }
        public File File { get; set; }
        public ThemeTemplate ThemeTemplate { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }

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