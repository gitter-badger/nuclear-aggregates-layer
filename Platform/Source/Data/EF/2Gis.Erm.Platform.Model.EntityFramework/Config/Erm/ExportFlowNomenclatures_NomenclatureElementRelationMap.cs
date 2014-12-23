using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ExportFlowNomenclatures_NomenclatureElementRelationMap : EntityConfig<ExportFlowNomenclatures_NomenclatureElementRelation, ErmContainer>
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