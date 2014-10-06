using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class LimitMap : EntityTypeConfiguration<Limit>
    {
        public LimitMap()
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

            Property(t => t.Comment)
                .HasMaxLength(255);

            // Table & Column Mappings
            ToTable("Limits", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ReplicationCode).HasColumnName("ReplicationCode");
            Property(t => t.AccountId).HasColumnName("AccountId");
            Property(t => t.CloseDate).HasColumnName("CloseDate");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.StartPeriodDate).HasColumnName("StartPeriodDate");
            Property(t => t.EndPeriodDate).HasColumnName("EndPeriodDate");
            Property(t => t.InspectorCode).HasColumnName("InspectorCode");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");
            Property(t => t.Comment).HasColumnName("Comment");

            // Relationships
            HasRequired(t => t.Account)
                .WithMany(t => t.Limits)
                .HasForeignKey(d => d.AccountId);
        }
    }
}