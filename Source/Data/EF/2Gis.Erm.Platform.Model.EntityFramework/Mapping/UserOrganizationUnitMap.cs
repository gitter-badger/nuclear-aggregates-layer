using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class UserOrganizationUnitMap : EntityTypeConfiguration<UserOrganizationUnit>
    {
        public UserOrganizationUnitMap()
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
            ToTable("UserOrganizationUnits", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // FIXME {a.tukaev, 03.10.2014}: Разобраться
            //// Relationships
            //this.HasRequired(t => t.OrganizationUnit)
            //    .WithMany(t => t.UserOrganizationUnits)
            //    .HasForeignKey(d => d.OrganizationUnitId);
            //this.HasRequired(t => t.User)
            //    .WithMany(t => t.UserOrganizationUnits)
            //    .HasForeignKey(d => d.UserId);
        }
    }
}