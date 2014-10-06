using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class NotificationProcessingMap : EntityTypeConfiguration<NotificationProcessings>
    {
        public NotificationProcessingMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Description)
                .HasMaxLength(4000);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("NotificationProcessings", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.EmailId).HasColumnName("EmailId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.AttemptsCount).HasColumnName("AttemptsCount");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.NotificationEmail)
                .WithMany(t => t.NotificationProcessings)
                .HasForeignKey(d => d.EmailId);
        }
    }
}