using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class TaskReferenceMap : EntityConfig<TaskReference, ErmContainer>
    {
        public TaskReferenceMap()
        {
            // Primary Key
            HasKey(t => new { t.TaskId, t.Reference, t.ReferencedType, t.ReferencedObjectId });

            // Properties
            Property(t => t.TaskId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Reference)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ReferencedObjectId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("TaskReferences", "Activity");
            Property(t => t.TaskId).HasColumnName("TaskId");
            Property(t => t.Reference).HasColumnName("Reference");
            Property(t => t.ReferencedType).HasColumnName("ReferencedType");
            Property(t => t.ReferencedObjectId).HasColumnName("ReferencedObjectId");

            // Relationships
            HasRequired(t => t.TaskBase)
                .WithMany(t => t.TaskReferences)
                .HasForeignKey(d => d.TaskId);
        }
    }
}