using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ThemeMap : EntityConfig<Theme, ErmContainer>
    {
        public ThemeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Themes", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ThemeTemplateId).HasColumnName("ThemeTemplateId");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.BeginDistribution).HasColumnName("BeginDistribution");
            Property(t => t.EndDistribution).HasColumnName("EndDistribution");
            Property(t => t.IsDefault).HasColumnName("IsDefault");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.File)
                .WithMany(t => t.Themes)
                .HasForeignKey(d => d.FileId);
            HasRequired(t => t.ThemeTemplate)
                .WithMany(t => t.Themes)
                .HasForeignKey(d => d.ThemeTemplateId);
        }
    }
}