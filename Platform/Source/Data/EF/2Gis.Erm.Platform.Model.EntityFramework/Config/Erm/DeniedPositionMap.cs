using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class DeniedPositionMap : EntityConfig<DeniedPosition, ErmContainer>
    {
        public DeniedPositionMap()
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
            ToTable("DeniedPositions", "Billing");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.PositionId).HasColumnName("PositionId");
            Property(t => t.PositionDeniedId).HasColumnName("PositionDeniedId");
            Property(t => t.PriceId).HasColumnName("PriceId");
            Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            Property(t => t.IsActive).HasColumnName("IsActive");
            Property(t => t.ObjectBindingType).HasColumnName("ObjectBindingType");            
            Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            Property(t => t.CreatedOn).HasColumnName("CreatedOn");
            Property(t => t.ModifiedOn).HasColumnName("ModifiedOn");
            Property(t => t.Timestamp).HasColumnName("Timestamp");

            // Relationships
            HasRequired(t => t.PositionDenied)
                .WithMany(t => t.DeniedPositions)
                .HasForeignKey(d => d.PositionDeniedId);
            HasRequired(t => t.Price)
                .WithMany(t => t.DeniedPositions)
                .HasForeignKey(d => d.PriceId);
        }
    }
}