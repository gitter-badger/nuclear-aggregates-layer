using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class LegalPersonMap : EntityConfig<LegalPerson, ErmContainer>
    {
        public LegalPersonMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.LegalName)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.ShortName)
                .HasMaxLength(256);

            Property(t => t.LegalAddress)
                .HasMaxLength(512);

            Property(t => t.Inn)
                .HasMaxLength(12);

            Property(t => t.VAT)
                .HasMaxLength(11);

            Property(t => t.Kpp)
                .HasMaxLength(12);

            Property(t => t.Ic)
                .HasMaxLength(8);

            Property(t => t.PassportSeries)
                .HasMaxLength(4);

            Property(t => t.PassportNumber)
                .HasMaxLength(9);

            Property(t => t.PassportIssuedBy)
                .HasMaxLength(512);

            Property(t => t.RegistrationAddress)
                .HasMaxLength(512);

            Property(t => t.CardNumber)
                .HasMaxLength(15);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("LegalPersons", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.IsInSyncWith1C).HasColumnName("IsInSyncWith1C");
            Property(t => t.ClientId).HasColumnName("ClientId");
            Property(t => t.LegalName).HasColumnName("LegalName");
            Property(t => t.ShortName).HasColumnName("ShortName");
            Property(t => t.LegalPersonTypeEnum).HasColumnName("LegalPersonTypeEnum");
            Property(t => t.LegalAddress).HasColumnName("LegalAddress");
            Property(t => t.Inn).HasColumnName("Inn");
            Property(t => t.VAT).HasColumnName("VAT");
            Property(t => t.Kpp).HasColumnName("Kpp");
            Property(t => t.Ic).HasColumnName("Ic");
            Property(t => t.PassportSeries).HasColumnName("PassportSeries");
            Property(t => t.PassportNumber).HasColumnName("PassportNumber");
            Property(t => t.PassportIssuedBy).HasColumnName("PassportIssuedBy");
            Property(t => t.RegistrationAddress).HasColumnName("RegistrationAddress");
            Property(t => t.CardNumber).HasColumnName("CardNumber");
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
            HasOptional(t => t.Client)
                .WithMany(t => t.LegalPersons)
                .HasForeignKey(d => d.ClientId);
        }
    }
}