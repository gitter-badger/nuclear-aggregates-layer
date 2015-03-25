using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class vwUserTerritoriesOrganizationUnitMap : EntityConfig<UserTerritoriesOrganizationUnits, ErmContainer>
    {
        public vwUserTerritoriesOrganizationUnitMap()
        {
            // Primary Key
            HasKey(t => new { t.UserId, t.OrganizationUnitId });

            // Properties
            Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.OrganizationUnitId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("vwUserTerritoriesOrganizationUnits", "Security");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.OrganizationUnitId).HasColumnName("OrganizationUnitId");
            Property(t => t.TerritoryId).HasColumnName("TerritoryId");
        }
    }
}