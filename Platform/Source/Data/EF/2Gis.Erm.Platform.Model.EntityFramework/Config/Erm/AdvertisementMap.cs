using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdvertisementMap : EntityConfig<Advertisement, ErmContainer>
    {
        public AdvertisementMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Advertisements", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FirmId).HasColumnName("FirmId");
            Property(t => t.AdvertisementTemplateId).HasColumnName("AdvertisementTemplateId");
            Property(t => t.IsSelectedToWhiteList).HasColumnName("IsSelectedToWhiteList");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.DgppId).HasColumnName("DgppId");

            // Relationships
            HasRequired(t => t.AdvertisementTemplate)
                .WithMany(t => t.Advertisements)
                .HasForeignKey(d => d.AdvertisementTemplateId);
            HasOptional(t => t.Firm)
                .WithMany(t => t.Advertisements)
                .HasForeignKey(d => d.FirmId);
        }
    }
}