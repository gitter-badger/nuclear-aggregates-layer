using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class FirmAddressServiceMap : EntityTypeConfiguration<FirmAddressService>
    {
        public FirmAddressServiceMap()
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
            ToTable("FirmAddressServices", "BusinessDirectory");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.FirmAddressId).HasColumnName("FirmAddressId");
            Property(t => t.ServiceId).HasColumnName("ServiceId");
            Property(t => t.DisplayService).HasColumnName("DisplayService");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.FirmAddress)
                .WithMany(t => t.FirmAddressServices)
                .HasForeignKey(d => d.FirmAddressId);
            HasRequired(t => t.AdditionalFirmService)
                .WithMany(t => t.FirmAddressServices)
                .HasForeignKey(d => d.ServiceId);
        }
    }
}