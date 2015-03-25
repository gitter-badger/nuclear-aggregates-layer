using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class ExportFailedEntityMap : EntityConfig<ExportFailedEntity, ErmContainer>
    {
        public ExportFailedEntityMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("ExportFailedEntities", "Integration");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.EntityName).HasColumnName("EntityName");
            Property(t => t.EntityId).HasColumnName("EntityId");
            Property(t => t.ProcessorId).HasColumnName("ProcessorId");
        }
    }
}