using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdvertisementTemplateMap : EntityConfig<AdvertisementTemplate, ErmContainer>
    {
        public AdvertisementTemplateMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("AdvertisementTemplates", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DummyAdvertisementId).HasColumnName("DummyAdvertisementId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsPublished).HasColumnName("IsPublished");
            Property(t => t.IsAllowedToWhiteList).HasColumnName("IsAllowedToWhiteList");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.IsAdvertisementRequired).HasColumnName("IsAdvertisementRequired");
        }
    }
}