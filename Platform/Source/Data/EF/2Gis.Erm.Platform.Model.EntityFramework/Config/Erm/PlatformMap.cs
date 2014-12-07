using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PlatformMap : EntityConfig<Entities.Erm.Platform, ErmContainer>
    {
        public PlatformMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Platforms", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.PlacementPeriodEnum).HasColumnName("PlacementPeriodEnum");
            Property(t => t.MinPlacementPeriodEnum).HasColumnName("MinPlacementPeriodEnum");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.IsSupportedByExport).HasColumnName("IsSupportedByExport");
        }
    }
}