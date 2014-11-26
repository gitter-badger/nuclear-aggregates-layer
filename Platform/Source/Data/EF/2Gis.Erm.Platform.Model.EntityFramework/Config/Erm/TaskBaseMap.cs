using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class TaskBaseMap : EntityConfig<TaskBase, ErmContainer>
    {
        public TaskBaseMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Subject)
                .HasMaxLength(256);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            Property(t => t.ScheduledOn)
                .HasColumnType("datetime2")
                .HasPrecision(7);


            Property(t => t.CreatedOn)
                .HasColumnType("datetime2")
                .HasPrecision(7);

            Property(t => t.ModifiedOn)
                .HasColumnType("datetime2")
                .HasPrecision(7);

            // Table & Column Mappings
            ToTable("TaskBase", "Activity");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Subject).HasColumnName("Subject");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.ScheduledOn).HasColumnName("ScheduledOn");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.TaskType).HasColumnName("TaskType");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
        }
    }
}