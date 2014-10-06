using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class ExportFlowNomenclatures_NomenclatureElementRelationMap : EntityTypeConfiguration<ExportFlowNomenclatures_NomenclatureElementRelation>
    {
        public ExportFlowNomenclatures_NomenclatureElementRelationMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ExportFlowNomenclatures_NomenclatureElementRelation", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Date).HasColumnName("Date");
        }
    }
}