using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ThemeOrganizationUnitMap : EntityConfig<ThemeOrganizationUnit, ErmContainer>
    {
        public ThemeOrganizationUnitMap()
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
            ToTable("ThemeOrganizationUnits", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ThemeId).HasColumnName("ThemeId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.ThemeOrganizationUnits)
                .HasForeignKey(d => d.OrganizationUnitId);
            HasRequired(t => t.Theme)
                .WithMany(t => t.ThemeOrganizationUnits)
                .HasForeignKey(d => d.ThemeId);
        }
    }
}