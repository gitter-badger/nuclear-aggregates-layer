using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class CategoryOrganizationUnitMap : EntityConfig<CategoryOrganizationUnit, ErmContainer>
    {
        public CategoryOrganizationUnitMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("CategoryOrganizationUnits", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.CategoryGroupId).HasColumnName("CategoryGroupId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.CategoryOrganizationUnits)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasRequired(t => t.Category)
                .WithMany(t => t.CategoryOrganizationUnits)
                .HasForeignKey(d => d.CategoryId);
            HasOptional(t => t.CategoryGroup)
                .WithMany(t => t.CategoryOrganizationUnits)
                .HasForeignKey(d => d.CategoryGroupId);
        }
    }
}