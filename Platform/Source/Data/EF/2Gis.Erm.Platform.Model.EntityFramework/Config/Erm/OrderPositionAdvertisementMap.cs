using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderPositionAdvertisementMap : EntityConfig<OrderPositionAdvertisement, ErmContainer>
    {
        public OrderPositionAdvertisementMap()
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
            ToTable("OrderPositionAdvertisement", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderPositionId).HasColumnName("OrderPositionId");
            Property(t => t.PositionId).HasColumnName("PositionId");
            Property(t => t.AdvertisementId).HasColumnName("AdvertisementId");
            Property(t => t.FirmAddressId).HasColumnName("FirmAddressId");
            Property(t => t.CategoryId).HasColumnName("CategoryId");
            Property(t => t.ThemeId).HasColumnName("ThemeId");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Advertisement)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.AdvertisementId);
            HasOptional(t => t.Category)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.CategoryId);
            HasOptional(t => t.FirmAddress)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.FirmAddressId);
            HasRequired(t => t.OrderPosition)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.OrderPositionId);
            HasRequired(t => t.Position)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.PositionId);
            HasOptional(t => t.Theme)
                .WithMany(t => t.OrderPositionAdvertisements)
                .HasForeignKey(d => d.ThemeId);
        }
    }
}