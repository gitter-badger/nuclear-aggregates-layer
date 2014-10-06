using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrdersRegionalAdvertisingSharingMap : EntityTypeConfiguration<OrdersRegionalAdvertisingSharing>
    {
        public OrdersRegionalAdvertisingSharingMap()
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
            ToTable("OrdersRegionalAdvertisingSharings", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.RegionalAdvertisingSharingId).HasColumnName("RegionalAdvertisingSharingId");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Order)
                .WithMany(t => t.OrdersRegionalAdvertisingSharings)
                .HasForeignKey(d => d.OrderId);
            HasRequired(t => t.RegionalAdvertisingSharing)
                .WithMany(t => t.OrdersRegionalAdvertisingSharings)
                .HasForeignKey(d => d.RegionalAdvertisingSharingId);
        }
    }
}