using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class AdditionalFirmServiceMap : EntityConfig<AdditionalFirmService, ErmContainer>
    {
        public AdditionalFirmServiceMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ServiceCode)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.Description)
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("AdditionalFirmServices", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ServiceCode).HasColumnName("ServiceCode");
            Property(t => t.IsManaged).HasColumnName("IsManaged");
            Property(t => t.Description).HasColumnName("Description");
        }
    }
}