using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderProcessingRequestMap : EntityConfig<OrderProcessingRequest, ErmContainer>
    {
        public OrderProcessingRequestMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("OrderProcessingRequests", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.RequestType).HasColumnName("RequestType");
            Property(t => t.DueDate).HasColumnName("DueDate");
            Property(t => t.BaseOrderId).HasColumnName("BaseOrderId");
            Property(t => t.RenewedOrderId).HasColumnName("RenewedOrderId");
            Property(t => t.ReleaseCountPlan).HasColumnName("ReleaseCountPlan");
            Property(t => t.SourceOrganizationUnitId).HasColumnName("SourceOrganizationUnitId");
            Property(t => t.BeginDistributionDate).HasColumnName("BeginDistributionDate");
            Property(t => t.FirmId).HasColumnName("FirmId");
            Property(t => t.LegalPersonProfileId).HasColumnName("LegalPersonProfileId");
            Property(t => t.LegalPersonId).HasColumnName("LegalPersonId");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.State).HasColumnName("State");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.LegalPersonProfile)
                .WithMany(t => t.OrderProcessingRequests)
                .HasForeignKey(d => d.LegalPersonProfileId);
            HasRequired(t => t.LegalPerson)
                .WithMany(t => t.OrderProcessingRequests)
                .HasForeignKey(d => d.LegalPersonId);
            HasOptional(t => t.BaseOrder)
                .WithMany(t => t.BaseOrderProcessingRequests)
                .HasForeignKey(d => d.BaseOrderId);
            HasRequired(t => t.Firm)
                .WithMany(t => t.OrderProcessingRequests)
                .HasForeignKey(d => d.FirmId);
            HasOptional(t => t.RenewedOrder)
                .WithMany(t => t.RenewedOrderProcessingRequests)
                .HasForeignKey(d => d.RenewedOrderId);
            HasRequired(t => t.SourceOrganizationUnit)
                .WithMany(t => t.OrderProcessingRequests)
                .HasForeignKey(d => d.SourceOrganizationUnitId);
        }
    }
}