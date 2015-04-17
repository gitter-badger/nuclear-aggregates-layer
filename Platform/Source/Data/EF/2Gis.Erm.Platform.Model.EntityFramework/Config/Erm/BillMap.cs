using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BillMap : EntityConfig<Bill, ErmContainer>
    {
        public BillMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(64);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Bills", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.Number).HasColumnName("Number");
            Property(t => t.BillDate).HasColumnName("BillDate");
            Property(t => t.BeginDistributionDate).HasColumnName("BeginDistributionDate");
            Property(t => t.EndDistributionDate).HasColumnName("EndDistributionDate");
            Property(t => t.PaymentDatePlan).HasColumnName("PaymentDatePlan");
            Property(t => t.PayablePlan).HasColumnName("PayablePlan");
            Property(t => t.VatPlan).HasColumnName("VatPlan");
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
                .WithMany(t => t.Bills)
                .HasForeignKey(d => d.OrderId);
        }
    }
}