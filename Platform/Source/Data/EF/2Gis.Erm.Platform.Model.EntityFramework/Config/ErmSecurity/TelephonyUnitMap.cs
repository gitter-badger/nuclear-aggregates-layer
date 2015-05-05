using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.ErmSecurity
{
    public class TelephonyUnitMap : EntityConfig<TelephonyUnit, ErmSecurityContainer>
    {
        public TelephonyUnitMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("TelephonyUnits", "Shared");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Address).HasColumnName("Address");
        }
    }
}
