using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AfterSaleServiceActivityMap : EntityConfig<AfterSaleServiceActivity, ErmContainer>
    {
        public AfterSaleServiceActivityMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("AfterSaleServiceActivities", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DealId).HasColumnName("DealId");
            Property(t => t.AfterSaleServiceType).HasColumnName("AfterSaleServiceType");
            Property(t => t.AbsoluteMonthNumber).HasColumnName("AbsoluteMonthNumber");

            // Relationships
            HasRequired(t => t.Deal)
                .WithMany(t => t.AfterSaleServiceActivities)
                .HasForeignKey(d => d.DealId);
        }
    }
}