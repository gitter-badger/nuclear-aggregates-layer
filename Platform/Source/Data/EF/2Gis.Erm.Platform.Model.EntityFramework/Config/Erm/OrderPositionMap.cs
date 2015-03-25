using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderPositionMap : EntityConfig<OrderPosition, ErmContainer>
    {
        public OrderPositionMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Comment)
                .HasMaxLength(300);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("OrderPositions", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.PricePositionId).HasColumnName("PricePositionId");
            Property(t => t.CategoryRate).HasColumnName("CategoryRate");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.PricePerUnit).HasColumnName("PricePerUnit");
            Property(t => t.PricePerUnitWithVat).HasColumnName("PricePerUnitWithVat");
            Property(t => t.DiscountSum).HasColumnName("DiscountSum");
            Property(t => t.DiscountPercent).HasColumnName("DiscountPercent");
            Property(t => t.CalculateDiscountViaPercent).HasColumnName("CalculateDiscountViaPercent");
            Property(t => t.PayablePrice).HasColumnName("PayablePrice");
            Property(t => t.PayablePlanWoVat).HasColumnName("PayablePlanWoVat");
            Property(t => t.PayablePlan).HasColumnName("PayablePlan");
            Property(t => t.ShipmentPlan).HasColumnName("ShipmentPlan");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Order)
                .WithMany(t => t.OrderPositions)
                .HasForeignKey(d => d.OrderId);
            HasRequired(t => t.PricePosition)
                .WithMany(t => t.OrderPositions)
                .HasForeignKey(d => d.PricePositionId);
        }
    }
}