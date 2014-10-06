using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrderValidationRuleGroupDetailMap : EntityTypeConfiguration<OrderValidationRuleGroupDetail>
    {
        public OrderValidationRuleGroupDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OrderValidationRuleGroupDetails", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderValidationGroupId).HasColumnName("OrderValidationGroupId");
            Property(t => t.RuleCode).HasColumnName("RuleCode");

            // Relationships
            HasRequired(t => t.OrderValidationRuleGroup)
                .WithMany(t => t.OrderValidationRuleGroupDetails)
                .HasForeignKey(d => d.OrderValidationGroupId);
        }
    }
}