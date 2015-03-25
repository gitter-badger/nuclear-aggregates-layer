using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AccountMap : EntityConfig<Account, ErmContainer>
    {
        public AccountMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LegalPesonSyncCode1C)
                .HasMaxLength(50);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Accounts", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.BranchOfficeOrganizationUnitId).HasColumnName("BranchOfficeOrganizationUnitId");
            Property(t => t.LegalPersonId).HasColumnName("LegalPersonId");
            Property(t => t.LegalPesonSyncCode1C).HasColumnName("LegalPesonSyncCode1C");
            Property(t => t.Balance).HasColumnName("Balance");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.BranchOfficeOrganizationUnit)
                .WithMany(t => t.Accounts)
                .HasForeignKey(d => d.BranchOfficeOrganizationUnitId);
            HasRequired(t => t.LegalPerson)
                .WithMany(t => t.Accounts)
                .HasForeignKey(d => d.LegalPersonId);
        }
    }
}