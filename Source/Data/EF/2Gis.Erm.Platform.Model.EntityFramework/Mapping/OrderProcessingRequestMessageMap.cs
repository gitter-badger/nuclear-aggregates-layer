using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrderProcessingRequestMessageMap : EntityTypeConfiguration<OrderProcessingRequestMessage>
    {
        public OrderProcessingRequestMessageMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.MessageParameters)
                .IsRequired();

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("OrderProcessingRequestMessages", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderRequestId).HasColumnName("OrderRequestId");
            Property(t => t.MessageType).HasColumnName("MessageType");
            Property(t => t.MessageTemplateCode).HasColumnName("MessageTemplateCode");
            Property(t => t.MessageParameters).HasColumnName("MessageParameters");
            Property(t => t.GroupId).HasColumnName("GroupId");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.OrderProcessingRequest)
                .WithMany(t => t.OrderProcessingRequestMessages)
                .HasForeignKey(d => d.OrderRequestId);
        }
    }
}