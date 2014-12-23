using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LegalPersonDealMap : EntityConfig<LegalPersonDeal, ErmContainer>
    {
        public LegalPersonDealMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("LegalPersonDeals", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.LegalPersonId).HasColumnName("LegalPersonId");
            Property(t => t.DealId).HasColumnName("DealId");
            Property(t => t.IsMain).HasColumnName("IsMain");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            HasRequired(t => t.Deal)
                .WithMany(t => t.LegalPersonDeals)
                .HasForeignKey(d => d.DealId);
            HasRequired(t => t.LegalPerson)
                .WithMany(t => t.LegalPersonDeals)
                .HasForeignKey(d => d.LegalPersonId);
        }
    }
}