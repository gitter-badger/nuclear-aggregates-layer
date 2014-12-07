using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ThemeTemplateMap : EntityConfig<ThemeTemplate, ErmContainer>
    {
        public ThemeTemplateMap()
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
            ToTable("ThemeTemplates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.TemplateCode).HasColumnName("TemplateCode");
            Property(t => t.IsSkyScraper).HasColumnName("IsSkyScraper");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.File)
                .WithMany(t => t.ThemeTemplates)
                .HasForeignKey(d => d.FileId);
        }
    }
}