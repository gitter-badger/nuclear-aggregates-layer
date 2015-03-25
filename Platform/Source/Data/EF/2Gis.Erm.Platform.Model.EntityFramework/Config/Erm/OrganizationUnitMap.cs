using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrganizationUnitMap : EntityConfig<OrganizationUnit, ErmContainer>
    {
        public OrganizationUnitMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SyncCode1C)
                .HasMaxLength(50);

            Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.ElectronicMedia)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("OrganizationUnits", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.SyncCode1C).HasColumnName("SyncCode1C");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.FirstEmitDate).HasColumnName("FirstEmitDate");
            Property(t => t.CountryId).HasColumnName("CountryId");
            Property(t => t.TimeZoneId).HasColumnName("TimeZoneId");
            Property(t => t.ErmLaunchDate).HasColumnName("ErmLaunchDate");
            Property(t => t.InfoRussiaLaunchDate).HasColumnName("InfoRussiaLaunchDate");
            Property(t => t.ElectronicMedia).HasColumnName("ElectronicMedia");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Country)
                .WithMany(t => t.OrganizationUnits)
                .HasForeignKey(d => d.CountryId);
        }
    }
}