using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class CityPhoneZoneMap : EntityConfig<CityPhoneZone, ErmContainer>
    {
        public CityPhoneZoneMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("CityPhoneZone", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CityCode).HasColumnName("CityCode");
            Property(t => t.IsDefault).HasColumnName("IsDefault");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}