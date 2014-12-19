using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class RolePrivilegeMap : EntityConfig<RolePrivilege, ErmSecurityContainer>
    {
        public RolePrivilegeMap()
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
            ToTable("RolePrivileges", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.RoleId).HasColumnName("RoleId");
            Property(t => t.PrivilegeId).HasColumnName("PrivilegeId");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.Mask).HasColumnName("Mask");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Privilege)
                .WithMany(t => t.RolePrivileges)
                .HasForeignKey(d => d.PrivilegeId);
            HasRequired(t => t.Role)
                .WithMany(t => t.RolePrivileges)
                .HasForeignKey(d => d.RoleId);
        }
    }
}