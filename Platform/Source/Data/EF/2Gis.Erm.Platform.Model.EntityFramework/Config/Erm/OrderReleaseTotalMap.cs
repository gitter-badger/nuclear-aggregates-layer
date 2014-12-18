using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderReleaseTotalMap : EntityConfig<OrderReleaseTotal, ErmContainer>
    {
        public OrderReleaseTotalMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OrderReleaseTotals", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.ReleaseNumber).HasColumnName("ReleaseNumber");
            Property(t => t.ReleaseBeginDate).HasColumnName("ReleaseBeginDate");
            Property(t => t.ReleaseEndDate).HasColumnName("ReleaseEndDate");
            Property(t => t.AmountToWithdraw).HasColumnName("AmountToWithdraw");
            Property(t => t.Vat).HasColumnName("Vat");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");

            // Relationships
            HasRequired(t => t.Order)
                .WithMany(t => t.OrderReleaseTotals)
                .HasForeignKey(d => d.OrderId);
        }
    }
}