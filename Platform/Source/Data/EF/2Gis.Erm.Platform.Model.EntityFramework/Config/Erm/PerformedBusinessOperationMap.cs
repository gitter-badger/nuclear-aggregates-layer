using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PerformedBusinessOperationMap : EntityConfig<PerformedBusinessOperation, ErmContainer>
    {
        public PerformedBusinessOperationMap()
        {
            // Primary Key
            HasKey(t => new { t.Id, t.Date });

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Context)
                .IsRequired();

            Property(t => t.OperationEntities)
                .HasMaxLength(1024);

            // Table & Column Mappings
            ToTable("PerformedBusinessOperations", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Operation).HasColumnName("Operation");
            Property(t => t.Descriptor).HasColumnName("Descriptor");
            Property(t => t.Context).HasColumnName("Context");
            Property(t => t.Date).HasColumnName("Date");
            Property(t => t.Parent).HasColumnName("Parent");
            Property(t => t.UseCaseId).HasColumnName("UseCaseId");
            Property(t => t.OperationEntities).HasColumnName("OperationEntities");
        }
    }
}