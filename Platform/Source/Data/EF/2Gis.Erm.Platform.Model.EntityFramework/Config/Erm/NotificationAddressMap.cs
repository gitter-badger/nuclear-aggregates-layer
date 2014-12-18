using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class NotificationAddressMap : EntityConfig<NotificationAddresses, ErmContainer>
    {
        public NotificationAddressMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Address)
                .IsRequired()
                .HasMaxLength(1024);

            Property(t => t.DisplayName)
                .HasMaxLength(1024);

            Property(t => t.DisplayNameEncoding)
                .HasMaxLength(1024);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("NotificationAddresses", "Shared");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.DisplayName).HasColumnName("DisplayName");
            Property(t => t.DisplayNameEncoding).HasColumnName("DisplayNameEncoding");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
        }
    }
}