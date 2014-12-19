using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderValidationResultMap : EntityConfig<OrderValidationResult, ErmContainer>
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
            Property(t => t.IsValid).HasColumnName("IsValid");
        }
    }
}