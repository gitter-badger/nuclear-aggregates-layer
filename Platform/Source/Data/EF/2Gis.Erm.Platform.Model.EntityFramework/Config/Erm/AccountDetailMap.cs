using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AccountDetailMap : EntityConfig<AccountDetail, ErmContainer>
    {
        public AccountDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Description)
                .HasMaxLength(200);

            Property(t => t.Comment)
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("AccountDetails", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.OperationTypeId).HasColumnName("OperationTypeId");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.TransactionDate).HasColumnName("TransactionDate");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Account)
                .WithMany(t => t.AccountDetails)
                .HasForeignKey(d => d.AccountId);
            HasRequired(t => t.OperationType)
                .WithMany(t => t.AccountDetails)
                .HasForeignKey(d => d.OperationTypeId);
        }
    }
}