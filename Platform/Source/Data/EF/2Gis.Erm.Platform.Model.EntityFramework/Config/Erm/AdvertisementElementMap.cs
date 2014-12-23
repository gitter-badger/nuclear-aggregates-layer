using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdvertisementElementMap : EntityConfig<AdvertisementElement, ErmContainer>
    {
        public AdvertisementElementMap()
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
            ToTable("AdvertisementElements", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AdvertisementId).HasColumnName("AdvertisementId");
            Property(t => t.AdvertisementElementTemplateId).HasColumnName("AdvertisementElementTemplateId");
            Property(t => t.AdsTemplatesAdsElementTemplatesId).HasColumnName("AdsTemplatesAdsElementTemplatesId");
            Property(t => t.Text).HasColumnName("Text");
            Property(t => t.FileId).HasColumnName("FileId");
            Property(t => t.BeginDate).HasColumnName("BeginDate");
            Property(t => t.EndDate).HasColumnName("EndDate");
            Property(t => t.FasCommentType).HasColumnName("FasCommentType");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.DgppId).HasColumnName("DgppId");

            // Relationships
            HasRequired(t => t.AdsTemplatesAdsElementTemplate)
                .WithMany(t => t.AdvertisementElements)
                .HasForeignKey(d => d.AdsTemplatesAdsElementTemplatesId);
            HasRequired(t => t.Advertisement)
                .WithMany(t => t.AdvertisementElements)
                .HasForeignKey(d => d.AdvertisementId);
            HasRequired(t => t.AdvertisementElementTemplate)
                .WithMany(t => t.AdvertisementElements)
                .HasForeignKey(d => d.AdvertisementElementTemplateId);
            HasOptional(t => t.File)
                .WithMany(t => t.AdvertisementElements)
                .HasForeignKey(d => d.FileId);
        }
    }
}