using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LegalPersonProfileMap : EntityConfig<LegalPersonProfile, ErmContainer>
    {
        public LegalPersonProfileMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.PositionInNominative)
                .HasMaxLength(256);

            Property(t => t.PositionInGenitive)
                .HasMaxLength(256);

            Property(t => t.Registered)
                .HasMaxLength(150);

            Property(t => t.ChiefNameInNominative)
                .HasMaxLength(256);

            Property(t => t.ChiefNameInGenitive)
                .HasMaxLength(256);

            Property(t => t.CertificateNumber)
                .HasMaxLength(50);

            Property(t => t.AccountNumber)
                .HasMaxLength(24);

            Property(t => t.BankCode)
                .HasMaxLength(4);

            Property(t => t.IBAN)
                .HasMaxLength(28);

            Property(t => t.SWIFT)
                .HasMaxLength(11);

            Property(t => t.BankName)
                .HasMaxLength(100);

            Property(t => t.BankAddress)
                .HasMaxLength(512);

            Property(t => t.WarrantyNumber)
                .HasMaxLength(50);

            Property(t => t.BargainNumber)
                .HasMaxLength(50);

            Property(t => t.DocumentsDeliveryAddress)
                .HasMaxLength(512);

            Property(t => t.PostAddress)
                .HasMaxLength(512);

            Property(t => t.RecipientName)
                .HasMaxLength(256);

            Property(t => t.EmailForAccountingDocuments)
                .HasMaxLength(64);

            Property(t => t.Email)
                .HasMaxLength(64);

            Property(t => t.PersonResponsibleForDocuments)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Phone)
                .HasMaxLength(50);

            Property(t => t.PaymentEssentialElements)
                .HasMaxLength(512);

            Property(t => t.RegistrationCertificateNumber)
                .HasMaxLength(9);

            Property(t => t.Timestamp)
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("LegalPersonProfiles", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.LegalPersonId).HasColumnName("LegalPersonId");
            Property(t => t.IsMainProfile).HasColumnName("IsMainProfile");
            Property(t => t.PositionInNominative).HasColumnName("PositionInNominative");
            Property(t => t.PositionInGenitive).HasColumnName("PositionInGenitive");
            Property(t => t.Registered).HasColumnName("Registered");
            Property(t => t.ChiefNameInNominative).HasColumnName("ChiefNameInNominative");
            Property(t => t.ChiefNameInGenitive).HasColumnName("ChiefNameInGenitive");
            Property(t => t.OperatesOnTheBasisInGenitive).HasColumnName("OperatesOnTheBasisInGenitive");
            Property(t => t.CertificateNumber).HasColumnName("CertificateNumber");
            Property(t => t.PaymentMethod).HasColumnName("PaymentMethod");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.BankCode).HasColumnName("BankCode");
            Property(t => t.IBAN).HasColumnName("IBAN");
            Property(t => t.SWIFT).HasColumnName("SWIFT");
            Property(t => t.BankName).HasColumnName("BankName");
            Property(t => t.BankAddress).HasColumnName("BankAddress");
            Property(t => t.CertificateDate).HasColumnName("CertificateDate");
            Property(t => t.WarrantyNumber).HasColumnName("WarrantyNumber");
            Property(t => t.WarrantyBeginDate).HasColumnName("WarrantyBeginDate");
            Property(t => t.WarrantyEndDate).HasColumnName("WarrantyEndDate");
            Property(t => t.BargainNumber).HasColumnName("BargainNumber");
            Property(t => t.BargainBeginDate).HasColumnName("BargainBeginDate");
            Property(t => t.BargainEndDate).HasColumnName("BargainEndDate");
            Property(t => t.DocumentsDeliveryAddress).HasColumnName("DocumentsDeliveryAddress");
            Property(t => t.PostAddress).HasColumnName("PostAddress");
            Property(t => t.RecipientName).HasColumnName("RecipientName");
            Property(t => t.DocumentsDeliveryMethod).HasColumnName("DocumentsDeliveryMethod");
            Property(t => t.EmailForAccountingDocuments).HasColumnName("EmailForAccountingDocuments");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.PersonResponsibleForDocuments).HasColumnName("PersonResponsibleForDocuments");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.PaymentEssentialElements).HasColumnName("PaymentEssentialElements");
            Property(t => t.RegistrationCertificateDate).HasColumnName("RegistrationCertificateDate");
            Property(t => t.RegistrationCertificateNumber).HasColumnName("RegistrationCertificateNumber");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.LegalPerson)
                .WithMany(t => t.LegalPersonProfiles)
                .HasForeignKey(d => d.LegalPersonId);
        }
    }
}