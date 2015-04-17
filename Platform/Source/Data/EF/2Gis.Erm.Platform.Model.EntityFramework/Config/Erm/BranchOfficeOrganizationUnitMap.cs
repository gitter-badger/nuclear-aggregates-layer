using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BranchOfficeOrganizationUnitMap : EntityConfig<BranchOfficeOrganizationUnit, ErmContainer>
    {
        public BranchOfficeOrganizationUnitMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SyncCode1C)
                .HasMaxLength(50);

            Property(t => t.RegistrationCertificate)
                .HasMaxLength(256);

            Property(t => t.ShortLegalName)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.ApplicationCityName)
                .HasMaxLength(256);

            Property(t => t.PositionInNominative)
                .HasMaxLength(256);

            Property(t => t.PositionInGenitive)
                .HasMaxLength(256);

            Property(t => t.ChiefNameInNominative)
                .HasMaxLength(256);

            Property(t => t.ChiefNameInGenitive)
                .HasMaxLength(256);

            Property(t => t.Registered)
                .HasMaxLength(150);

            Property(t => t.OperatesOnTheBasisInGenitive)
                .HasMaxLength(256);

            Property(t => t.Kpp)
                .HasMaxLength(15);

            Property(t => t.PaymentEssentialElements)
                .HasMaxLength(256);

            Property(t => t.PhoneNumber)
                .HasMaxLength(50);

            Property(t => t.ActualAddress)
                .IsRequired()
                .HasMaxLength(512);

            Property(t => t.PostalAddress)
                .HasMaxLength(512);

            Property(t => t.Email)
                .HasMaxLength(64);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("BranchOfficeOrganizationUnits", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.SyncCode1C).HasColumnName("SyncCode1C");
            Property(t => t.RegistrationCertificate).HasColumnName("RegistrationCertificate");
            Property(t => t.BranchOfficeId).HasColumnName("BranchOfficeId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.ShortLegalName).HasColumnName("ShortLegalName");
            Property(t => t.ApplicationCityName).HasColumnName("ApplicationCityName");
            Property(t => t.PositionInNominative).HasColumnName("PositionInNominative");
            Property(t => t.PositionInGenitive).HasColumnName("PositionInGenitive");
            Property(t => t.ChiefNameInNominative).HasColumnName("ChiefNameInNominative");
            Property(t => t.ChiefNameInGenitive).HasColumnName("ChiefNameInGenitive");
            Property(t => t.Registered).HasColumnName("Registered");
            Property(t => t.OperatesOnTheBasisInGenitive).HasColumnName("OperatesOnTheBasisInGenitive");
            Property(t => t.Kpp).HasColumnName("Kpp");
            Property(t => t.PaymentEssentialElements).HasColumnName("PaymentEssentialElements");
            Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            Property(t => t.ActualAddress).HasColumnName("ActualAddress");
            Property(t => t.PostalAddress).HasColumnName("PostalAddress");
            Property(t => t.Email).HasColumnName("Email");
            Property(t => t.IsPrimary).HasColumnName("IsPrimary");
            Property(t => t.IsPrimaryForRegionalSales).HasColumnName("IsPrimaryForRegionalSales");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.BranchOffice)
                .WithMany(t => t.BranchOfficeOrganizationUnits)
                .HasForeignKey(d => d.BranchOfficeId);
            HasRequired(t => t.OrganizationUnit)
                .WithMany(t => t.BranchOfficeOrganizationUnits)
                .HasForeignKey(d => d.OrganizationUnitId);
        }
    }
}