using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class vwOrganizationUnitMap : EntityTypeConfiguration<OrganizationUnitDto>
    {
        public vwOrganizationUnitMap()
        {
            // Primary Key
            HasKey(t => new { t.Id, t.Name, t.Code, t.CreatedBy, t.CreatedOn });

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(5);

            Property(t => t.CreatedBy)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("vwOrganizationUnits", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
        }
    }
}