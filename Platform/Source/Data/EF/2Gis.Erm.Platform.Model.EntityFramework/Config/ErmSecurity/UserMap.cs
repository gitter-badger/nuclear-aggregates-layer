using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class UserMap : EntityConfig<User, ErmSecurityContainer>
    {
        public UserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Account)
                .IsRequired()
                .HasMaxLength(250);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            Property(t => t.LastName)
                .HasMaxLength(100);

            Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(250);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Users", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Account).HasColumnName("Account");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.DisplayName).HasColumnName("DisplayName");
            Property(t => t.DepartmentId).HasColumnName("DepartmentId");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.IsServiceUser).HasColumnName("IsServiceUser");

            // Relationships
            HasRequired(t => t.Department)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.DepartmentId);
            HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(d => d.ParentId);
        }
    }
}