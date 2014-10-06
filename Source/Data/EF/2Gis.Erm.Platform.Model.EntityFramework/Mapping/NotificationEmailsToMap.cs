using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class NotificationEmailsToMap : EntityTypeConfiguration<NotificationEmailsTo>
    {
        public NotificationEmailsToMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("NotificationEmailsTo", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.EmailId).HasColumnName("EmailId");
            Property(t => t.AddressId).HasColumnName("AddressId");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.NotificationAddress)
                .WithMany(t => t.NotificationEmailsToes)
                .HasForeignKey(d => d.AddressId);
            HasRequired(t => t.NotificationEmails)
                .WithMany(t => t.NotificationEmailsTo)
                .HasForeignKey(d => d.EmailId);
        }
    }
}