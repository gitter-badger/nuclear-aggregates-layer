using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class DealMap : EntityConfig<Deal, ErmContainer>
    {
        public DealMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(300);

            Property(t => t.CloseReasonOther)
                .HasMaxLength(256);

            Property(t => t.Comment)
                .HasMaxLength(512);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("Deals", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.MainFirmId).HasColumnName("MainFirmId");
            Property(t => t.ClientId).HasColumnName("ClientId");
            Property(t => t.CurrencyId).HasColumnName("CurrencyId");
            Property(t => t.StartReason).HasColumnName("StartReason");
            Property(t => t.CloseReason).HasColumnName("CloseReason");
            Property(t => t.CloseReasonOther).HasColumnName("CloseReasonOther");
            Property(t => t.CloseDate).HasColumnName("CloseDate");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.DealStage).HasColumnName("DealStage");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Client)
                .WithMany(t => t.Deals)
                .HasForeignKey(d => d.ClientId);
            HasRequired(t => t.Currency)
                .WithMany(t => t.Deals)
                .HasForeignKey(d => d.CurrencyId);
            HasOptional(t => t.Firm)
                .WithMany(t => t.Deals)
                .HasForeignKey(d => d.MainFirmId);
        }
    }
}