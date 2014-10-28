using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class RegionalAdvertisingSharingMap : EntityConfig<RegionalAdvertisingSharing, ErmContainer>
    {
        public RegionalAdvertisingSharingMap()
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
            ToTable("RegionalAdvertisingSharings", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.BeginDistributionDate).HasColumnName("BeginDistributionDate");
            Property(t => t.SourceOrganizationUnitId).HasColumnName("SourceOrganizationUnitId");
            Property(t => t.DestOrganizationUnitId).HasColumnName("DestOrganizationUnitId");
            Property(t => t.SourceBranchOfficeOrganizationUnitId).HasColumnName("SourceBranchOfficeOrganizationUnitId");
            Property(t => t.DestBranchOfficeOrganizationUnitId).HasColumnName("DestBranchOfficeOrganizationUnitId");
            Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.SourceBranchOfficeOrganizationUnit)
                .WithMany(t => t.RegionalAdvertisingSharings)
                .HasForeignKey(d => d.SourceBranchOfficeOrganizationUnitId);
            HasRequired(t => t.DestBranchOfficeOrganizationUnit)
                .WithMany(t => t.RegionalAdvertisingSharings1)
                .HasForeignKey(d => d.DestBranchOfficeOrganizationUnitId);
            HasRequired(t => t.SourceOrganizationUnit)
                .WithMany(t => t.RegionalAdvertisingSharings)
                .HasForeignKey(d => d.SourceOrganizationUnitId);
            HasRequired(t => t.DestOrganizationUnit)
                .WithMany(t => t.RegionalAdvertisingSharings1)
                .HasForeignKey(d => d.DestOrganizationUnitId);
        }
    }
}