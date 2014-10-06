using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrderValidationResultMap : EntityTypeConfiguration<OrderValidationResult>
    {
        public OrderValidationResultMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OrderValidationResults", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.OrderValidationGroupId).HasColumnName("OrderValidationGroupId");
            Property(t => t.OrderValidationType).HasColumnName("OrderValidationType");
            Property(t => t.IsValid).HasColumnName("IsValid");

            // Relationships
            HasRequired(t => t.Order)
                .WithMany(t => t.OrderValidationResults)
                .HasForeignKey(d => d.OrderId);
        }
    }
}