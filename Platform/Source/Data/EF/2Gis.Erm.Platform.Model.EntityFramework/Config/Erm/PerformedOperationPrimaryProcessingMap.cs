using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PerformedOperationPrimaryProcessingMap : EntityConfig<PerformedOperationPrimaryProcessing, ErmContainer>
    {
        public PerformedOperationPrimaryProcessingMap()
        {
            // Primary Key
            HasKey(t => new { t.UseCaseId, t.MessageFlowId });

            // Properties
            // Table & Column Mappings
            ToTable("PerformedOperationPrimaryProcessings", "Shared");
            Property(t => t.UseCaseId).HasColumnName("UseCaseId");
            Property(t => t.MessageFlowId).HasColumnName("MessageFlowId");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.AttemptCount).HasColumnName("AttemptCount");
            Property(t => t.LastProcessedOn).HasColumnName("LastProcessedOn");
        }
    }
}