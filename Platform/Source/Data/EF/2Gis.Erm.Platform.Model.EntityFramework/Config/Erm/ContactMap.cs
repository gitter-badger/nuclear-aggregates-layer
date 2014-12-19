using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ContactMap : EntityConfig<Contact, ErmContainer>
    {
        public ContactMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.FullName)
                .IsRequired()
                .HasMaxLength(160);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(160);

            Property(t => t.MiddleName)
                .HasMaxLength(160);

            Property(t => t.LastName)
                .HasMaxLength(160);

            Property(t => t.Salutation)
                .HasMaxLength(160);

            Property(t => t.MainPhoneNumber)
                .HasMaxLength(64);

            Property(t => t.AdditionalPhoneNumber)
                .HasMaxLength(64);

            Property(t => t.MobilePhoneNumber)
                .HasMaxLength(64);

            Property(t => t.HomePhoneNumber)
                .HasMaxLength(64);

            Property(t => t.Fax)
                .HasMaxLength(50);

            Property(t => t.WorkEmail)
                .HasMaxLength(100);

            Property(t => t.HomeEmail)
                .HasMaxLength(100);

            Property(t => t.Website)
                .HasMaxLength(200);

            Property(t => t.ImIdentifier)
                .HasMaxLength(64);

            Property(t => t.JobTitle)
                .HasMaxLength(170);

            Property(t => t.Department)
                .HasMaxLength(100);

            Property(t => t.WorkAddress)
                .HasMaxLength(450);

            Property(t => t.HomeAddress)
                .HasMaxLength(450);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Contacts", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.FullName).HasColumnName("FullName");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.MiddleName).HasColumnName("MiddleName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Salutation).HasColumnName("Salutation");
            Property(t => t.GenderCode).HasColumnName("GenderCode");
            Property(t => t.FamilyStatusCode).HasColumnName("FamilyStatusCode");
            Property(t => t.MainPhoneNumber).HasColumnName("MainPhoneNumber");
            Property(t => t.AdditionalPhoneNumber).HasColumnName("AdditionalPhoneNumber");
            Property(t => t.MobilePhoneNumber).HasColumnName("MobilePhoneNumber");
            Property(t => t.HomePhoneNumber).HasColumnName("HomePhoneNumber");
            Property(t => t.Fax).HasColumnName("Fax");
            Property(t => t.WorkEmail).HasColumnName("WorkEmail");
            Property(t => t.HomeEmail).HasColumnName("HomeEmail");
            Property(t => t.Website).HasColumnName("Website");
            Property(t => t.ImIdentifier).HasColumnName("ImIdentifier");
            Property(t => t.ClientId).HasColumnName("ClientId");
            Property(t => t.JobTitle).HasColumnName("JobTitle");
            Property(t => t.AccountRole).HasColumnName("AccountRole");
            Property(t => t.Department).HasColumnName("Department");
            Property(t => t.IsFired).HasColumnName("IsFired");
            Property(t => t.BirthDate).HasColumnName("BirthDate");
            Property(t => t.WorkAddress).HasColumnName("WorkAddress");
            Property(t => t.HomeAddress).HasColumnName("HomeAddress");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Client)
                .WithMany(t => t.Contacts)
                .HasForeignKey(d => d.ClientId);
        }
    }
}