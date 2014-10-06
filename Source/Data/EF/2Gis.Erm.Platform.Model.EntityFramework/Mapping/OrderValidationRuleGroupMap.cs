using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrderValidationRuleGroupMap : EntityTypeConfiguration<OrderValidationRuleGroup>
    {
        public OrderValidationRuleGroupMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OrderValidationRuleGroups", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Code).HasColumnName("Code");
        }
    }
}