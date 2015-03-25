using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class NotificationEmailMap : EntityConfig<NotificationEmails, ErmContainer>
    {
        public NotificationEmailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(4000);

            Property(t => t.SubjectEncoding)
                .HasMaxLength(128);

            Property(t => t.Body)
                .IsRequired();

            Property(t => t.BodyEncoding)
                .HasMaxLength(128);

            Property(t => t.Priority)
                .HasMaxLength(128);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("NotificationEmails", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SenderId).HasColumnName("SenderId");
            Property(t => t.Subject).HasColumnName("Subject");
            Property(t => t.SubjectEncoding).HasColumnName("SubjectEncoding");
            Property(t => t.Body).HasColumnName("Body");
            Property(t => t.BodyEncoding).HasColumnName("BodyEncoding");
            Property(t => t.IsBodyHtml).HasColumnName("IsBodyHtml");
            Property(t => t.ExpirationTime).HasColumnName("ExpirationTime");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.MaxAttemptsCount).HasColumnName("MaxAttemptsCount");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasOptional(t => t.Sender)
                .WithMany(t => t.NotificationEmails)
                .HasForeignKey(d => d.SenderId);
        }
    }
}