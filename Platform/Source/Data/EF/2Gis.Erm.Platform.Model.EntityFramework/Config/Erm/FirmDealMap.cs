using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class FirmDealMap : EntityConfig<FirmDeal, ErmContainer>
    {
        public FirmDealMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("FirmDeals", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FirmId).HasColumnName("FirmId");
            Property(t => t.DealId).HasColumnName("DealId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            HasRequired(t => t.Deal)
                .WithMany(t => t.FirmDeals)
                .HasForeignKey(d => d.DealId);
            HasRequired(t => t.Firm)
                .WithMany(t => t.FirmDeals)
                .HasForeignKey(d => d.FirmId);

        }
    }
}
