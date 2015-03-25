using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PerformedOperationFinalProcessingMap : EntityConfig<PerformedOperationFinalProcessing, ErmContainer>
    {
        public PerformedOperationFinalProcessingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("PerformedOperationFinalProcessings", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MessageFlowId).HasColumnName("MessageFlowId");
            Property(t => t.EntityTypeId).HasColumnName("EntityTypeId");
            Property(t => t.EntityId).HasColumnName("EntityId");
            Property(t => t.Context).HasColumnName("Context");
            Property(t => t.AttemptCount).HasColumnName("AttemptCount");
            Property(t => t.OperationId).HasColumnName("OperationId");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
        }
    }
}