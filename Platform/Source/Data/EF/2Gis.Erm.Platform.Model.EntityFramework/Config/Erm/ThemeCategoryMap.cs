using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ThemeCategoryMap : EntityConfig<ThemeCategory, ErmContainer>
    {
        public ThemeCategoryMap()
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
            ToTable("ThemeCategories", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ThemeId).HasColumnName("ThemeId");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Category)
                .WithMany(t => t.ThemeCategories)
                .HasForeignKey(d => d.CategoryId);
            HasRequired(t => t.Theme)
                .WithMany(t => t.ThemeCategories)
                .HasForeignKey(d => d.ThemeId);
        }
    }
}