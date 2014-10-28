using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class OrderMap : EntityConfig<Order, ErmContainer>
    {
        public OrderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.RegionalNumber)
                .HasMaxLength(200);

            Property(t => t.DiscountComment)
                .HasMaxLength(300);

            Property(t => t.DocumentsComment)
                .HasMaxLength(300);

            Property(t => t.Comment)
                .HasMaxLength(300);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Orders", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Number).HasColumnName("Number");
            Property(t => t.RegionalNumber).HasColumnName("RegionalNumber");
            Property(t => t.FirmId).HasColumnName("FirmId");
            Property(t => t.SourceOrganizationUnitId).HasColumnName("SourceOrganizationUnitId");
            Property(t => t.DestOrganizationUnitId).HasColumnName("DestOrganizationUnitId");
            Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.BeginDistributionDate).HasColumnName("BeginDistributionDate");
            Property(t => t.EndDistributionDatePlan).HasColumnName("EndDistributionDatePlan");
            Property(t => t.EndDistributionDateFact).HasColumnName("EndDistributionDateFact");
            Property(t => t.BeginReleaseNumber).HasColumnName("BeginReleaseNumber");
            Property(t => t.EndReleaseNumberPlan).HasColumnName("EndReleaseNumberPlan");
            Property(t => t.EndReleaseNumberFact).HasColumnName("EndReleaseNumberFact");
            Property(t => t.ReleaseCountPlan).HasColumnName("ReleaseCountPlan");
            Property(t => t.ReleaseCountFact).HasColumnName("ReleaseCountFact");
            Property(t => t.LegalPersonId).HasColumnName("LegalPersonId");
            Property(t => t.BranchOfficeOrganizationUnitId).HasColumnName("BranchOfficeOrganizationUnitId");
            Property(t => t.WorkflowStepId).HasColumnName("WorkflowStepId");
            Property(t => t.DiscountReasonEnum).HasColumnName("DiscountReasonEnum");
            Property(t => t.DiscountComment).HasColumnName("DiscountComment");
            Property(t => t.ApprovalDate).HasColumnName("ApprovalDate");
            Property(t => t.RejectionDate).HasColumnName("RejectionDate");
            Property(t => t.SignupDate).HasColumnName("SignupDate");
            Property(t => t.IsTerminated).HasColumnName("IsTerminated");
            Property(t => t.DealId).HasColumnName("DealId");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.BargainId).HasColumnName("BargainId");
            Property(t => t.HasDocumentsDebt).HasColumnName("HasDocumentsDebt");
            Property(t => t.DocumentsComment).HasColumnName("DocumentsComment");
            Property(t => t.TechnicallyTerminatedOrderId).HasColumnName("TechnicallyTerminatedOrderId");
            Property(t => t.InspectorCode).HasColumnName("InspectorCode");
            Property(t => t.AmountWithdrawn).HasColumnName("AmountWithdrawn");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.PlatformId).HasColumnName("PlatformId");
            Property(t => t.TerminationReason).HasColumnName("TerminationReason");
            Property(t => t.OrderType).HasColumnName("OrderType");
            Property(t => t.PayableFact).HasColumnName("PayableFact");
            Property(t => t.PayablePlan).HasColumnName("PayablePlan");
            Property(t => t.PayablePrice).HasColumnName("PayablePrice");
            Property(t => t.DiscountSum).HasColumnName("DiscountSum");
            Property(t => t.AmountToWithdraw).HasColumnName("AmountToWithdraw");
            Property(t => t.VatPlan).HasColumnName("VatPlan");
            Property(t => t.DiscountPercent).HasColumnName("DiscountPercent");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.PaymentMethod).HasColumnName("PaymentMethod");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.LegalPersonProfileId).HasColumnName("LegalPersonProfileId");

            // Relationships
            HasOptional(t => t.Account)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.AccountId);
            HasOptional(t => t.Bargain)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.BargainId);
            HasOptional(t => t.BranchOfficeOrganizationUnit)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.BranchOfficeOrganizationUnitId);
            HasOptional(t => t.Currency)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.CurrencyId);
            HasOptional(t => t.Deal)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.DealId);
            HasOptional(t => t.LegalPersonProfile)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.LegalPersonProfileId);
            HasOptional(t => t.LegalPerson)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.LegalPersonId);
            HasRequired(t => t.DestOrganizationUnit)
                .WithMany(t => t.OrdersByDestination)
                .HasForeignKey(d => d.DestOrganizationUnitId);
            HasRequired(t => t.Firm)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.FirmId);
            HasOptional(t => t.TechnicallyTerminatedOrder)
                .WithMany(t => t.ReplacingOrders)
                .HasForeignKey(d => d.TechnicallyTerminatedOrderId);
            HasOptional(t => t.Platform)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.PlatformId);
            HasRequired(t => t.DestOrganizationUnit)
                .WithMany(t => t.OrdersByDestination)
                .HasForeignKey(d => d.SourceOrganizationUnitId);
        }
    }
}