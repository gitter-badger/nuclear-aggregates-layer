using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class vwSecurityAcceleratorMap : EntityTypeConfiguration<SecurityAccelerator>
    {
        public vwSecurityAcceleratorMap()
        {
            // Primary Key
            HasKey(t => new { t.UserId, t.DepartmentId });

            // Properties
            Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.DepartmentId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("vwSecurityAccelerator", "Security");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.DepartmentId).HasColumnName("DepartmentId");
            Property(t => t.DepartmentLeftBorder).HasColumnName("DepartmentLeftBorder");
            Property(t => t.DepartmentRightBorder).HasColumnName("DepartmentRightBorder");
        }
    }
}