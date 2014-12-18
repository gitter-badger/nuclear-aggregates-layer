using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LockMap : EntityConfig<Lock, ErmContainer>
    {
        public LockMap()
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
            ToTable("Locks", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.PeriodStartDate).HasColumnName("PeriodStartDate");
            Property(t => t.PeriodEndDate).HasColumnName("PeriodEndDate");
            Property(t => t.CloseDate).HasColumnName("CloseDate");
            Property(t => t.PlannedAmount).HasColumnName("PlannedAmount");
            Property(t => t.Balance).HasColumnName("Balance");
            Property(t => t.ClosedBalance).HasColumnName("ClosedBalance");
            Property(t => t.DebitAccountDetailId).HasColumnName("DebitAccountDetailId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.AccountDetail)
                .WithMany(t => t.Locks)
                .HasForeignKey(d => d.DebitAccountDetailId);
            HasRequired(t => t.Account)
                .WithMany(t => t.Locks)
                .HasForeignKey(d => d.AccountId);
            HasRequired(t => t.Order)
                .WithMany(t => t.Locks)
                .HasForeignKey(d => d.OrderId);
        }
    }
}