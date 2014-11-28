using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class BranchOfficeMap : EntityConfig<BranchOffice, ErmContainer>
    {
        public BranchOfficeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.LegalAddress)
                .IsRequired()
                .HasMaxLength(512);

            Property(t => t.Inn)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.Ic)
                .HasMaxLength(8);

            Property(t => t.Annotation)
                .HasMaxLength(512);

            Property(t => t.UsnNotificationText)
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("BranchOffices", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.LegalAddress).HasColumnName("LegalAddress");
            Property(t => t.Inn).HasColumnName("Inn");
            Property(t => t.Ic).HasColumnName("Ic");
            Property(t => t.BargainTypeId).HasColumnName("BargainTypeId");
            Property(t => t.ContributionTypeId).HasColumnName("ContributionTypeId");
            Property(t => t.Annotation).HasColumnName("Annotation");
            Property(t => t.UsnNotificationText).HasColumnName("UsnNotificationText");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.BargainType)
                .WithMany(t => t.BranchOffices)
                .HasForeignKey(d => d.BargainTypeId);
            HasOptional(t => t.ContributionType)
                .WithMany(t => t.BranchOffices)
                .HasForeignKey(d => d.ContributionTypeId);
        }
    }
}