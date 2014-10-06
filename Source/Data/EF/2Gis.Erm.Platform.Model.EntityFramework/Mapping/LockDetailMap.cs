using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Mapping
{
    public class LockDetailMap : EntityTypeConfiguration<LockDetail>
    {
        public LockDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Description)
                .HasMaxLength(200);

            Property(t => t.Timestamp)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            ToTable("LockDetails", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.PriceId).HasColumnName("PriceId");
            Property(t => t.OrderPositionId).HasColumnName("OrderPositionId");
            Property(t => t.ChargeSessionId).HasColumnName("ChargeSessionId");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.LockId).HasColumnName("LockId");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Lock)
                .WithMany(t => t.LockDetails)
                .HasForeignKey(d => d.LockId);
            HasRequired(t => t.Price)
                .WithMany(t => t.LockDetails)
                .HasForeignKey(d => d.PriceId);
        }
    }
}