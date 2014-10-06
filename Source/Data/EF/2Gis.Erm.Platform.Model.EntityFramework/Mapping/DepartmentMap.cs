using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class DepartmentMap : EntityTypeConfiguration<Department>
    {
        public DepartmentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Departments", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ParentId).HasColumnName("ParentId");
            Property(t => t.LeftBorder).HasColumnName("LeftBorder");
            Property(t => t.RightBorder).HasColumnName("RightBorder");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Parent)
                .WithMany(t => t.Children)
                .HasForeignKey(d => d.ParentId);
        }
    }
}