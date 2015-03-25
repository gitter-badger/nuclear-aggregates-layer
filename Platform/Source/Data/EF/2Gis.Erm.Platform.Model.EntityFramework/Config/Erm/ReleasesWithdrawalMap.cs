using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ReleasesWithdrawalMap : EntityConfig<ReleaseWithdrawal, ErmContainer>
    {
        public ReleasesWithdrawalMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("ReleasesWithdrawals", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderPositionId).HasColumnName("OrderPositionId");
            Property(t => t.ReleaseNumber).HasColumnName("ReleaseNumber");
            Property(t => t.ReleaseBeginDate).HasColumnName("ReleaseBeginDate");
            Property(t => t.ReleaseEndDate).HasColumnName("ReleaseEndDate");
            Property(t => t.AmountToWithdraw).HasColumnName("AmountToWithdraw");
            Property(t => t.Vat).HasColumnName("Vat");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.OrderPosition)
                .WithMany(t => t.ReleasesWithdrawals)
                .HasForeignKey(d => d.OrderPositionId);
        }
    }
}