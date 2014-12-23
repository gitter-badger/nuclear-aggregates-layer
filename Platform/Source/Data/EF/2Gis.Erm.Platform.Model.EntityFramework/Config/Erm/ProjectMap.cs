using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ProjectMap : EntityConfig<Project, ErmContainer>
    {
        public ProjectMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.NameLat)
                .HasMaxLength(160);

            Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(160);

            Property(t => t.DefaultLang)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.Timestamp)
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Projects", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.NameLat).HasColumnName("NameLat");
            Property(t => t.DisplayName).HasColumnName("DisplayName");
            Property(t => t.DefaultLang).HasColumnName("DefaultLang");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.OrganizationUnit)
                .WithMany(t => t.Projects)
                .HasForeignKey(d => d.OrganizationUnitId);
        }
    }
}