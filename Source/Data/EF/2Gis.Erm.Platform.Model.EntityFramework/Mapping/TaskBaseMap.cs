using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class TaskBaseMap : EntityTypeConfiguration<TaskBase>
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

            // Table & Column Mappings
            ToTable("TaskBase", "Activity");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Subject).HasColumnName("Subject");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.ScheduledStart).HasColumnName("ScheduledStart");
            Property(t => t.ScheduledEnd).HasColumnName("ScheduledEnd");
            Property(t => t.ActualEnd).HasColumnName("ActualEnd");
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