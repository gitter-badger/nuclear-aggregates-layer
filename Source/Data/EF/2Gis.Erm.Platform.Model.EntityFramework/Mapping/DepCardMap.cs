using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class DepCardMap : EntityTypeConfiguration<DepCard>
    {
        public DepCardMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("DepCards", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.IsHiddenOrArchived).HasColumnName("IsHiddenOrArchived");
        }
    }
}