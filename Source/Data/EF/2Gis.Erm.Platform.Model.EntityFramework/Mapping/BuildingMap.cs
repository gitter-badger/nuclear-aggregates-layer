using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class BuildingMap : EntityTypeConfiguration<Building>
    {
        public BuildingMap()
        {
            // Primary Key
            HasKey(t => t.Code);

            // Properties
            Property(t => t.Code)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("Buildings", "Integration");
            Property(t => t.Code).HasColumnName("Code");
            Property(t => t.TerritoryId).HasColumnName("TerritoryId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");

            // Relationships
            HasRequired(t => t.Territory)
                .WithMany(t => t.Buildings)
                .HasForeignKey(d => d.TerritoryId);
        }
    }
}