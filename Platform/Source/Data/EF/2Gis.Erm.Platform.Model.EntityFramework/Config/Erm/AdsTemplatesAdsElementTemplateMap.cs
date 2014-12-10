using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdsTemplatesAdsElementTemplateMap : EntityConfig<AdsTemplatesAdsElementTemplate, ErmContainer>
    {
        public AdsTemplatesAdsElementTemplateMap()
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
            ToTable("AdsTemplatesAdsElementTemplates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AdsTemplateId).HasColumnName("AdsTemplateId");
            Property(t => t.AdsElementTemplateId).HasColumnName("AdsElementTemplateId");
            Property(t => t.ExportCode).HasColumnName("ExportCode");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.AdvertisementElementTemplate)
                .WithMany(t => t.AdsTemplatesAdsElementTemplates)
                .HasForeignKey(d => d.AdsElementTemplateId);
            HasRequired(t => t.AdvertisementTemplate)
                .WithMany(t => t.AdsTemplatesAdsElementTemplates)
                .HasForeignKey(d => d.AdsTemplateId);
        }
    }
}