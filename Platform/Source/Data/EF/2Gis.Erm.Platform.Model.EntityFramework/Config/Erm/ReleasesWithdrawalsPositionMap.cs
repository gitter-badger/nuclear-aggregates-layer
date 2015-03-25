using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ReleasesWithdrawalsPositionMap : EntityConfig<ReleasesWithdrawalsPosition, ErmContainer>
    {
        public ReleasesWithdrawalsPositionMap()
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
            ToTable("ReleasesWithdrawalsPositions", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReleasesWithdrawalId).HasColumnName("ReleasesWithdrawalId");
            Property(t => t.PositionId).HasColumnName("PositionId");
            Property(t => t.PlatformId).HasColumnName("PlatformId");
            Property(t => t.AmountToWithdraw).HasColumnName("AmountToWithdraw");
            Property(t => t.Vat).HasColumnName("Vat");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Platform)
                .WithMany(t => t.ReleasesWithdrawalsPositions)
                .HasForeignKey(d => d.PlatformId);
            HasRequired(t => t.Position)
                .WithMany(t => t.ReleasesWithdrawalsPositions)
                .HasForeignKey(d => d.PositionId);
            HasRequired(t => t.ReleasesWithdrawal)
                .WithMany(t => t.ReleasesWithdrawalsPositions)
                .HasForeignKey(d => d.ReleasesWithdrawalId);
        }
    }
}