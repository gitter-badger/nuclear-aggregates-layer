using System.ComponentModel.DataAnnotations.Schema;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.EntityFramework.Config.Erm
{
    public class OrderValidationCacheEntryMap : EntityConfig<OrderValidationCacheEntry, ErmContainer>
    {
        public OrderValidationCacheEntryMap()
        {
            // Primary Key
            HasKey(t => new { t.OrderId, t.ValidatorId, t.ValidVersion, t.OperationId });

            // Properties
            Property(t => t.OrderId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ValidatorId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ValidVersion)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8);

            // Table & Column Mappings
            ToTable("OrderValidationCacheEntries", "Shared");
            Property(t => t.OrderId).HasColumnName("OrderId");
            Property(t => t.ValidatorId).HasColumnName("ValidatorId");
            Property(t => t.ValidVersion).HasColumnName("ValidVersion");
            Property(t => t.OperationId).HasColumnName("OperationId");
        }
    }
}
