using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class PricePositionMap : EntityConfig<PricePosition, ErmContainer>
    {
        public PricePositionMap()
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
            ToTable("PricePositions", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.DgppId).HasColumnName("DgppId");
            Property(t => t.PriceId).HasColumnName("PriceId");
            Property(t => t.PositionId).HasColumnName("PositionId");
            Property(t => t.Cost).HasColumnName("Cost");
            Property(t => t.MinAdvertisementAmount).HasColumnName("MinAdvertisementAmount");
            Property(t => t.MaxAdvertisementAmount).HasColumnName("MaxAdvertisementAmount");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.AmountSpecificationMode).HasColumnName("AmountSpecificationMode");
            Property(t => t.RateType).HasColumnName("RateType");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.OwnerCode).HasColumnName("OwnerCode");
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.Position)
                .WithMany(t => t.PricePositions)
                .HasForeignKey(d => d.PositionId);
            HasRequired(t => t.Price)
                .WithMany(t => t.PricePositions)
                .HasForeignKey(d => d.PriceId);
        }
    }
}