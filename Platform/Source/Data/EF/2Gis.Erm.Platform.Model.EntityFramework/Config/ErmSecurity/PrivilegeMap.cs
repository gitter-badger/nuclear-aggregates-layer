using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class PrivilegeMap : EntityConfig<Privilege, ErmSecurityContainer>
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