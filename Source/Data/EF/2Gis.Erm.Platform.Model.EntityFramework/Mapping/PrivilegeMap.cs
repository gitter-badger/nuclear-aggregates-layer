using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class PrivilegeMap : EntityTypeConfiguration<Privilege>
    {
        public PrivilegeMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("Privileges", "Security");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.EntityType).HasColumnName("EntityType");
            Property(t => t.Operation).HasColumnName("Operation");
        }
    }
}