using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class PhonecallBaseMap : EntityConfig<PhonecallBase, ErmContainer>
    {
        public PhonecallBaseMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Subject)
                .HasMaxLength(256);

            Property(t => t.PhoneNumber)
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("PhonecallBase", "Activity");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Subject).HasColumnName("Subject");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.ScheduledStart).HasColumnName("ScheduledStart");
            Property(t => t.ScheduledEnd).HasColumnName("ScheduledEnd");
            Property(t => t.ActualEnd).HasColumnName("ActualEnd");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Direction).HasColumnName("Direction");
            Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            Property(t => t.Purpose).HasColumnName("Purpose");
            Property(t => t.AfterSaleType).HasColumnName("AfterSaleType");
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